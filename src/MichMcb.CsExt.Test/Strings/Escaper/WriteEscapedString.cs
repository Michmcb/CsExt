namespace MichMcb.CsExt.Test.Strings.Escaper
{
	using MichMcb.CsExt.Strings;
	using System.Text;
	using Xunit;

	public sealed class WriteEscapedString
	{
		[Fact]
		public void EscapeJsonString()
		{
			StringEscaper se = StringEscaper.Json();
			StringBuilder sb = new StringBuilder();
			se.WriteEscapedString(@"Path\To\Stuff", sb);
			Assert.Equal(@"Path\\To\\Stuff", sb.ToString());
			sb.Clear();

			se.WriteEscapedString("\"\\'\b\f\n\r\t", sb);
			Assert.Equal(@"\""\\'\b\f\n\r\t", sb.ToString());
			sb.Clear();
		}
	}
}
