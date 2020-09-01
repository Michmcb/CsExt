namespace MichMcb.CsExt.Test.Data.Base64
{
	using MichMcb.CsExt.Data;
	using System;
	using Xunit;
	public sealed class Decode
	{
		[Fact]
		public void OnlyPadding()
		{
			Base64 b64 = new Base64();
			Span<byte> decoded = stackalloc byte[1];
			Opt<int> written = b64.Decode("===", decoded);
			Assert.True(written.Ok);
			Assert.Equal(0, written.Val);
		}
		[Fact]
		public void Decoding_1to1()
		{
			Base64 b64 = new Base64();
			Span<byte> decoded = stackalloc byte[1];
			Opt<int> written = b64.Decode("T", decoded);
			Assert.True(written.Ok);
			Assert.Equal(1, written.Val);
			Assert.Equal(76, decoded[0]);
		}
		[Fact]
		public void Decoding_2to1()
		{
			Base64 b64 = new Base64();
			Span<byte> decoded = stackalloc byte[1];
			Opt<int> written = b64.Decode("TQ", decoded);
			Assert.True(written.Ok);
			Assert.Equal(1, written.Val);
			Assert.Equal(77, decoded[0]);
		}
		[Fact]
		public void Decoding_2to1WithPadding()
		{
			Base64 b64 = new Base64();
			Span<byte> decoded = stackalloc byte[1];
			Opt<int> written = b64.Decode("TQ==", decoded);
			Assert.Equal(1, written.Val);
			Assert.Equal(77, decoded[0]);
		}
		[Fact]
		public void Decoding_3to2()
		{
			Base64 b64 = new Base64();
			Span<byte> decoded = stackalloc byte[2];
			Opt<int> written = b64.Decode("TWE", decoded);
			Assert.True(written.Ok);
			Assert.Equal(2, written.Val);
			Assert.Equal(77, decoded[0]);
			Assert.Equal(97, decoded[1]);
		}
		[Fact]
		public void Decoding_3to2WithPadding()
		{
			Base64 b64 = new Base64();
			Span<byte> decoded = stackalloc byte[2];
			Opt<int> written = b64.Decode("TWE=", decoded);
			Assert.True(written.Ok);
			Assert.Equal(2, written.Val);
			Assert.Equal(77, decoded[0]);
			Assert.Equal(97, decoded[1]);
		}
		[Fact]
		public void Decoding_4to3()
		{
			Base64 b64 = new Base64();
			Span<byte> decoded = stackalloc byte[3];
			Opt<int> written = b64.Decode("TWFu", decoded);
			Assert.True(written.Ok);
			Assert.Equal(3, written.Val);
			Assert.Equal(77, decoded[0]);
			Assert.Equal(97, decoded[1]);
			Assert.Equal(110, decoded[2]);
		}
		[Fact]
		public void Decoding_5to4()
		{
			Base64 b64 = new Base64();
			Span<byte> decoded = stackalloc byte[4];
			Opt<int> written = b64.Decode("TWFuT", decoded);
			Assert.True(written.Ok);
			Assert.Equal(4, written.Val);
			Assert.Equal(77, decoded[0]);
			Assert.Equal(97, decoded[1]);
			Assert.Equal(110, decoded[2]);
			Assert.Equal(76, decoded[3]);
		}
		[Fact]
		public void Decoding_6to4()
		{
			Base64 b64 = new Base64();
			Span<byte> decoded = stackalloc byte[4];
			Opt<int> written = b64.Decode("TWFuTQ", decoded);
			Assert.True(written.Ok);
			Assert.Equal(4, written.Val);
			Assert.Equal(77, decoded[0]);
			Assert.Equal(97, decoded[1]);
			Assert.Equal(110, decoded[2]);
			Assert.Equal(77, decoded[3]);
		}
		[Fact]
		public void Decoding_7to5()
		{
			Base64 b64 = new Base64();
			Span<byte> decoded = stackalloc byte[5];
			Opt<int> written = b64.Decode("TWFuTWE", decoded);
			Assert.True(written.Ok);
			Assert.Equal(5, written.Val);
			Assert.Equal(77, decoded[0]);
			Assert.Equal(97, decoded[1]);
			Assert.Equal(110, decoded[2]);
			Assert.Equal(77, decoded[3]);
			Assert.Equal(97, decoded[4]);
		}
		[Fact]
		public void Decoding_8to6()
		{
			Base64 b64 = new Base64();
			Span<byte> decoded = stackalloc byte[6];
			Opt<int> written = b64.Decode("TWFuTWFu", decoded);
			Assert.True(written.Ok);
			Assert.Equal(6, written.Val);
			Assert.Equal(77, decoded[0]);
			Assert.Equal(97, decoded[1]);
			Assert.Equal(110, decoded[2]);
			Assert.Equal(77, decoded[3]);
			Assert.Equal(97, decoded[4]);
			Assert.Equal(110, decoded[5]);
		}
	}
	public sealed class Encode
	{
		[Fact]
		public void Encoding_1to2()
		{
			Base64 b64 = new Base64();
			Span<char> encoded = stackalloc char[2];
			int written = b64.Encode(new byte[] { 77, }, encoded);
			Assert.Equal(2, written);
			Assert.Equal('T', encoded[0]);
			Assert.Equal('Q', encoded[1]);
		}
		[Fact]
		public void Encoding_2to3()
		{
			Base64 b64 = new Base64();
			Span<char> encoded = stackalloc char[3];
			int written = b64.Encode(new byte[] { 77, 97 }, encoded);
			Assert.Equal(3, written);
			Assert.Equal('T', encoded[0]);
			Assert.Equal('W', encoded[1]);
			Assert.Equal('E', encoded[2]);
		}
		[Fact]
		public void Encoding_3to4()
		{
			Base64 b64 = new Base64();
			Span<char> encoded = stackalloc char[4];
			int written = b64.Encode(new byte[] { 77, 97, 110 }, encoded);
			Assert.Equal(4, written);
			Assert.Equal('T', encoded[0]);
			Assert.Equal('W', encoded[1]);
			Assert.Equal('F', encoded[2]);
			Assert.Equal('u', encoded[3]);
		}
		[Fact]
		public void Encoding_4to6()
		{
			Base64 b64 = new Base64();
			Span<char> encoded = stackalloc char[6];
			int written = b64.Encode(new byte[] { 77, 97, 110, 32 }, encoded);
			Assert.Equal(6, written);
			Assert.Equal('T', encoded[0]);
			Assert.Equal('W', encoded[1]);
			Assert.Equal('F', encoded[2]);
			Assert.Equal('u', encoded[3]);
			Assert.Equal('I', encoded[4]);
			Assert.Equal('A', encoded[5]);
		}
		[Fact]
		public void Encoding_5to7()
		{
			Base64 b64 = new Base64();
			Span<char> encoded = stackalloc char[7];
			int written = b64.Encode(new byte[] { 77, 97, 110, 32, 105 }, encoded);
			Assert.Equal(7, written);
			Assert.Equal('T', encoded[0]);
			Assert.Equal('W', encoded[1]);
			Assert.Equal('F', encoded[2]);
			Assert.Equal('u', encoded[3]);
			Assert.Equal('I', encoded[4]);
			Assert.Equal('G', encoded[5]);
			Assert.Equal('k', encoded[6]);
		}
		[Fact]
		public void Encoding_6to8()
		{
			Base64 b64 = new Base64();
			Span<char> encoded = stackalloc char[8];
			int written = b64.Encode(new byte[] { 77, 97, 110, 32, 105, 115 }, encoded);
			Assert.Equal(8, written);
			Assert.Equal('T', encoded[0]);
			Assert.Equal('W', encoded[1]);
			Assert.Equal('F', encoded[2]);
			Assert.Equal('u', encoded[3]);
			Assert.Equal('I', encoded[4]);
			Assert.Equal('G', encoded[5]);
			Assert.Equal('l', encoded[6]);
			Assert.Equal('z', encoded[7]);
		}
	}
}
