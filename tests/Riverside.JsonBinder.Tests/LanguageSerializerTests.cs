using System.Text.Json.Nodes;
using Riverside.JsonBinder.Serialization;

namespace Riverside.JsonBinder.Tests;

[TestClass]
public class LanguageSerializerTests
{
	private static readonly Dictionary<SerializableLanguage, Type> SerializerTypes = new()
	{
		{ SerializableLanguage.CSharp, typeof(CSharpSerializer) },
		{ SerializableLanguage.Python, typeof(PythonSerializer) },
		{ SerializableLanguage.Java, typeof(JavaSerializer) },
		{ SerializableLanguage.JavaScript, typeof(JavaScriptSerializer) },
		{ SerializableLanguage.TypeScript, typeof(TypeScriptSerializer) },
		{ SerializableLanguage.PHP, typeof(PHPSerializer) },
		{ SerializableLanguage.Ruby, typeof(RubySerializer) },
		{ SerializableLanguage.Swift, typeof(SwiftSerializer) }
	};

	[TestMethod]
	[DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
	public void GenerateClasses_ValidJson_ReturnsExpectedResult(SerializableLanguage language, string json, string expected)
	{
		// Arrange
		if (!SerializerTypes.TryGetValue(language, out var serializerType) || serializerType == null)
		{
			Assert.Fail($"Serializer for language {language} not found.");
			return;
		}

		var serializer = (LanguageSerializer?)Activator.CreateInstance(serializerType);
		Assert.IsNotNull(serializer, $"Failed to create instance of {serializerType}");

		// Act
		var jsonNode = JsonNode.Parse(json);
		Assert.IsNotNull(jsonNode, "Failed to parse JSON");

		var result = serializer.GenerateClasses(jsonNode, "Root");

		// Assert
		Assert.AreEqual(expected.Normalize(), result.Normalize());
	}

	[TestMethod]
	[DynamicData(nameof(GetInvalidJsonTestData), DynamicDataSourceType.Method)]
	public void GenerateClasses_InvalidJson_ThrowsException(SerializableLanguage language, string invalidJson)
	{
		// Arrange
		if (!SerializerTypes.TryGetValue(language, out var serializerType) || serializerType == null)
		{
			Assert.Fail($"Serializer for language {language} not found.");
			return;
		}

		var serializer = (LanguageSerializer?)Activator.CreateInstance(serializerType);
		Assert.IsNotNull(serializer, $"Failed to create instance of {serializerType}");

		// Act
		try
		{
			var jsonNode = JsonNode.Parse(invalidJson);
			Assert.IsNotNull(jsonNode, "Failed to parse JSON");

			serializer.GenerateClasses(jsonNode, "Root");
		}
		catch (Exception ex)
		{
			// Assert
			Assert.AreEqual("'i' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 0.", ex.Message);
		}
	}

	public static IEnumerable<object[]> GetTestData()
	{
		yield return new object[] { SerializableLanguage.CSharp, "{\"name\":\"John\"}", "public class Root\n{\n    public string Name { get; set; }\n}" };
		yield return new object[] { SerializableLanguage.Python, "{\"name\":\"John\"}", "class Root:\n    def __init__(self):\n        self.name: str = None" };
		yield return new object[] { SerializableLanguage.Java, "{\"name\":\"John\"}", "public class Root {\n    private String name;\n\n    public String getname() { return name; }\n    public void setname(String name) { this.name = name; }\n}" };
		yield return new object[] { SerializableLanguage.JavaScript, "{\"name\":\"John\"}", "class Root {\n    constructor() {\n        this.name = null;\n    }\n}" };
		yield return new object[] { SerializableLanguage.TypeScript, "{\"name\":\"John\"}", "class Root {\n    constructor() {\n        this.name = null;\n    }\n}" };
		yield return new object[] { SerializableLanguage.PHP, "{\"name\":\"John\"}", "class Root {\n    public $name;\n}" };
		yield return new object[] { SerializableLanguage.Ruby, "{\"name\":\"John\"}", "class Root\n    attr_accessor :name\nend" };
		yield return new object[] { SerializableLanguage.Swift, "{\"name\":\"John\"}", "struct Root {\n    var name: String?\n}" };
	}

	public static IEnumerable<object[]> GetInvalidJsonTestData()
	{
		yield return new object[] { SerializableLanguage.CSharp, "invalid json" };
		yield return new object[] { SerializableLanguage.Python, "invalid json" };
		yield return new object[] { SerializableLanguage.Java, "invalid json" };
		yield return new object[] { SerializableLanguage.JavaScript, "invalid json" };
		yield return new object[] { SerializableLanguage.TypeScript, "invalid json" };
		yield return new object[] { SerializableLanguage.PHP, "invalid json" };
		yield return new object[] { SerializableLanguage.Ruby, "invalid json" };
		yield return new object[] { SerializableLanguage.Swift, "invalid json" };
	}
}
