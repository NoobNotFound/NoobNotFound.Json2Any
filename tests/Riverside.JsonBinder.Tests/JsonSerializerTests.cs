namespace Riverside.JsonBinder.Tests;

[TestClass]
public class JsonSerializerTests
{
	[TestMethod]
	[DataRow("{\"name\":\"John\"}", SerializableLanguage.CSharp, "public class Root\n{\n    public string name { get; set; }\n}")]
	[DataRow("{\"name\":\"John\"}", SerializableLanguage.Python, "class Root:\n    name: Optional[str]\n\n    def __init__(self):\n        self.name: str = None")]
	[DataRow("{\"name\":\"John\"}", SerializableLanguage.Java, "public class Root {\n    private String name;\n\n    public String getname() { return name; }\n    public void setname(String name) { this.name = name; }\n}")]
	public void ConvertTo_ValidJson_ReturnsExpectedResult(string json, SerializableLanguage language, string expected)
	{
		// Act
		var result = JsonSerializer.ConvertTo(json, language);

		// Assert
		Assert.AreEqual(Normalize(expected), Normalize(result));
	}

	[TestMethod]
	public void ConvertTo_InvalidJson_ThrowsException()
	{
		// Arrange
		string invalidJson = "invalid json";

		// Act
		try
		{
			// Stupidly, JsonReaderException is internal, so it can't be caught.
			// Instead, we need to compare the exception message with the expected message.
			JsonSerializer.ConvertTo(invalidJson, SerializableLanguage.CSharp);
		}
		catch (Exception ex)
		{
			// Assert
			Assert.AreEqual("'i' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 0.", ex.Message);
		}
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void ConvertTo_NullJson_ThrowsException()
	{
		// Arrange
		string? invalidJson = null;

		JsonSerializer.ConvertTo(invalidJson, SerializableLanguage.CSharp);
	}

	[TestMethod]
	[ExpectedException(typeof(NotSupportedException))]
	public void ConvertTo_UnsupportedLanguage_ThrowsNotSupportedException()
	{
		// Arrange
		string json = "{\"name\":\"John\"}";

		// Act
		JsonSerializer.ConvertTo(json, (SerializableLanguage)0xFFFFFF);
	}

	[TestMethod]
	public void ConvertTo_JsonArray_WrapsInRootObject()
	{
		// Arrange
		string jsonArray = "[{\"name\":\"John\"}]";
		string expected = """
			public class Root
			{
			    public List<ItemsItem> Items { get; set; }
			}

			public class ItemsItem
			{
			    public string Name { get; set; }
			}
			""";
		// Act
		var result = JsonSerializer.ConvertTo(jsonArray, SerializableLanguage.CSharp);

		// Assert
		Assert.AreEqual(Normalize(expected), Normalize(result));
	}

	private static string Normalize(string input)
		=> input.Replace("\r\n", "\n").Trim();
}
