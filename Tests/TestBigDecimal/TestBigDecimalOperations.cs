﻿// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "TestBigDecimalOperations.cs" last touched on 2021-05-14 at 7:56 AM by Protiguous.

namespace TestBigDecimal {

	using System;
	using System.Numerics;
	using Librainian.Maths.Bigger;
	using NUnit.Framework;

	[TestFixture]
	public class TestBigDecimalOperations {

		[Test]
		public void TestAddition() {
			var number1 = BigDecimal.Parse( "1234567890" );
			var expectedResult = BigDecimal.Parse( "3382051537" );

			var result = number1 + 2147483647;

			Assert.AreEqual( expectedResult, result );
		}

		[Test]
		public void TestBigDecimalPow() {
			var expectedResult = BigDecimal.Parse( "268637376395543538746286686601216000000000000" );

			// 5040 ^ 12  =  268637376395543538746286686601216000000000000

			var number = BigDecimal.Parse( "5040" );
			var result = BigDecimal.Pow( number, 12 );

			Assert.AreEqual( expectedResult, result );
		}

		[Test]
		public void TestCeiling001() {
			const String expectedCeiling = "4";
			const String expectedStart = "3.14159265";

			var start = BigDecimal.Parse( expectedStart );
			var actualStart = start.ToString();

			Assert.AreEqual( expectedStart, actualStart );

			var ceiling = BigDecimal.Ceiling( start );
			var actualCeiling = ceiling.ToString();

			Assert.AreEqual( expectedCeiling, actualCeiling );
		}

		[Test]
		public void TestCeiling002() {
			const String expectedCeiling = "-3";
			const String expectedStart = "-3.14159265";

			var start = BigDecimal.Parse( expectedStart );
			var actualStart = start.ToString();

			Assert.AreEqual( expectedStart, actualStart );

			var ceiling = BigDecimal.Ceiling( start );
			var actualCeiling = ceiling.ToString();

			Assert.AreEqual( expectedCeiling, actualCeiling );
		}

		[Test]
		public void TestCeiling003() {
			const String expectedCeiling = "1";
			const String expectedStart = "0.14159265";

			var start = BigDecimal.Parse( expectedStart );
			var actualStart = start.ToString();

			Assert.AreEqual( expectedStart, actualStart );

			var ceiling = BigDecimal.Ceiling( start );
			var actualCeiling = ceiling.ToString();

			Assert.AreEqual( expectedCeiling, actualCeiling );
		}

		[Test]
		public void TestCeiling004() {
			const String expectedCeiling = "0";
			const String expectedStart = "-0.14159265";

			var start = BigDecimal.Parse( expectedStart );
			var actualStart = start.ToString();

			Assert.AreEqual( expectedStart, actualStart );

			var ceiling = BigDecimal.Ceiling( start );
			var actualCeiling = ceiling.ToString();

			Assert.AreEqual( expectedCeiling, actualCeiling );
		}

		[Test]
		public void TestDivide000() {
			var expectedResult = BigDecimal.Parse( "7" );

			var dividend = BigDecimal.Parse( "0.63" );
			var divisor = BigDecimal.Parse( "0.09" );

			var result = BigDecimal.Divide( dividend, divisor );

			var expected = expectedResult.ToString();
			var actual = result.ToString();

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestDivide001() {
			var expectedResult = BigDecimal.Parse( "40094690950920881030683735292761468389214899724061" );

			var dividend = BigDecimal.Parse( "1522605027922533360535618378132637429718068114961380688657908494580122963258952897654000350692006139" );
			var divisor = BigDecimal.Parse( "37975227936943673922808872755445627854565536638199" );

			var result = BigDecimal.Divide( dividend, divisor );

			var expected = expectedResult.ToString();
			var actual = result.ToString();

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestDivide002() {
			const String expectedResultDividend = "0.001";
			const String expectedResultDivisor = "0.5";

			const String expectedQuotientResult = "0.002";

			var resultDividend = BigDecimal.Parse( expectedResultDividend );
			var resultDivisor = BigDecimal.Parse( expectedResultDivisor );

			//resultDividend.Normalize();
			//resultDivisor.Normalize();

			var quotientResult = BigDecimal.Divide( resultDividend, resultDivisor );

			//quotientResult.Normalize();

			var actualDividend = resultDividend.ToString();
			var actualDivisor = resultDivisor.ToString();
			var actualQuotientResult = quotientResult.ToString();

			Assert.AreEqual( expectedResultDividend, actualDividend );
			Assert.AreEqual( expectedResultDivisor, actualDivisor );
			Assert.AreEqual( expectedQuotientResult, actualQuotientResult );
		}

		[Test]
		public void TestDivide003() {
			const String expected = "1.1036742134828557";

			var divisor = BigDecimal.Parse( "0.90606447789" );
			var result = BigDecimal.Divide( BigDecimal.One, divisor );

			//result.Truncate( 100 );
			var actual = result.ToString();

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestDivide004() {
			var expectedResult = BigDecimal.Parse( "0.05" );

			var one = new BigDecimal( 1 );
			var twenty = new BigDecimal( 20 );
			var result = BigDecimal.Divide( one, twenty );

			var expected = expectedResult.ToString();
			var actual = result.ToString();

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestDivide005A() {
			var expectedResult3 = BigDecimal.Parse( "50" );

			var result3 = BigDecimal.Divide( BigDecimal.Parse( "0.5" ), BigDecimal.Parse( "0.01" ) );

			var expected3 = expectedResult3.ToString();
			var actual3 = result3.ToString();

			Assert.AreEqual( expected3, actual3 );
		}

		[Test]
		public void TestDivide005B() {
			var expectedResult3 = BigDecimal.Parse( "5" );

			var result3 = BigDecimal.Divide( BigDecimal.Parse( "0.5" ), BigDecimal.Parse( "0.1" ) );

			var expected3 = expectedResult3.ToString();
			var actual3 = result3.ToString();

			Assert.AreEqual( expected3, actual3 );
		}

		[Test]
		public void TestDivide005C() {
			var expectedResult3 = BigDecimal.Parse( "5" );

			var result3 = BigDecimal.Divide( BigDecimal.Parse( "0.05" ), BigDecimal.Parse( "0.01" ) );

			var expected3 = expectedResult3.ToString();
			var actual3 = result3.ToString();

			Assert.AreEqual( expected3, actual3 );
		}

		[Test]
		public void TestDivide005D() {
			var expectedResult3 = BigDecimal.Parse( "0.5" );

			var result3 = BigDecimal.Divide( BigDecimal.Parse( "0.05" ), BigDecimal.Parse( "0.1" ) );

			var expected3 = expectedResult3.ToString();
			var actual3 = result3.ToString();

			Assert.AreEqual( expected3, actual3 );
		}

		[Test]
		public void TestFloor001() {
			const String expectedFloor = "3";
			const String expectedStart = "3.14159265";

			var start = BigDecimal.Parse( expectedStart );
			var actualStart = start.ToString();

			Assert.AreEqual( expectedStart, actualStart );

			var floor = BigDecimal.Floor( start );
			var actualFloor = floor.ToString();

			Assert.AreEqual( expectedFloor, actualFloor );
		}

		[Test]
		public void TestFloor002() {
			const String expectedFloor = "-4";
			const String expectedStart = "-3.14159265";

			var start = BigDecimal.Parse( expectedStart );
			var actualStart = start.ToString();

			Assert.AreEqual( expectedStart, actualStart );

			var floor = BigDecimal.Floor( start );
			var actualFloor = floor.ToString();

			Assert.AreEqual( expectedFloor, actualFloor );
		}

		[Test]
		public void TestFloor003() {
			const String expectedFloor = "-1";
			const String expectedStart = "-0.14159265";

			var start = BigDecimal.Parse( expectedStart );
			var actualStart = start.ToString();

			Assert.AreEqual( expectedStart, actualStart );

			var floor = BigDecimal.Floor( start );
			var actualFloor = floor.ToString();

			Assert.AreEqual( expectedFloor, actualFloor );
		}

		[Test]
		public void TestFloor004() {
			const String expectedFloor = "0";
			const String expectedStart = "0.14159265";

			var start = BigDecimal.Parse( expectedStart );
			var actualStart = start.ToString();

			Assert.AreEqual( expectedStart, actualStart );

			var floor = BigDecimal.Floor( start );
			var actualFloor = floor.ToString();

			Assert.AreEqual( expectedFloor, actualFloor );
		}

		[Test]
		public void TestMod() {
			BigDecimal expectedResult1 = 12;
			BigDecimal expectedResult2 = 0;
			BigDecimal expectedResult3 = 1;

			//BigDecimal expectedResult4 = 1.66672;

			// 31 % 19 = 12
			BigDecimal dividend1 = 31;
			BigDecimal divisor1 = 19;

			// 1891 %31 = 0
			BigDecimal dividend2 = 1891;
			BigDecimal divisor2 = 31;

			// 6661 % 60 = 1
			BigDecimal dividend3 = 6661;
			BigDecimal divisor3 = 60;

			// 31 % 3.66666 = 1.66672
			//BigDecimal dividend4 = 31;
			//BigDecimal divisor4 = 3.66666;

			var result1 = BigDecimal.Mod( dividend1, divisor1 );
			var result2 = BigDecimal.Mod( dividend2, divisor2 );
			var result3 = BigDecimal.Mod( dividend3, divisor3 );

			//BigDecimal result4 = BigDecimal.Mod(dividend4,divisor4);

			Assert.AreEqual( expectedResult1, result1 );
			Assert.AreEqual( expectedResult2, result2 );
			Assert.AreEqual( expectedResult3, result3 );

			//Assert.AreEqual(expectedResult4, result4);
		}

		[Test]
		public void TestMultiply() {
			var expectedResult1 = BigDecimal.Parse( "35794234179725868774991807832568455403003778024228226193532908190484670252364677411513516111204504060317568667" );
			var expectedResult2 = BigDecimal.Parse( "37484040009320200288159018961010536937973891182532366282540247408867702983313960194873589374267102044942786001" );
			var expectedResult3 =
				new BigDecimal( BigInteger.Negate( BigInteger.Parse( "61199804023616162130466158636504166524066189692091806226423722790866248079929810268920239053350152436663869784" ) ) );

			//"6119980402361616213046615863650416652406618969209180622642372279086624807992.9810268920239053350152436663869784"

			//expectedResult3.Truncate();
			//expectedResult3.Normalize();

			var p = BigDecimal.Parse( "6122421090493547576937037317561418841225758554253106999" );
			var q = BigDecimal.Parse( "5846418214406154678836553182979162384198610505601062333" );

			var result1 = BigDecimal.Multiply( p, q );
			var result2 = p * p;
			var result3 = -1 * p * new BigDecimal( BigInteger.Parse( "9996013524558575221488141657137307396757453940901242216" ), -34 );

			// -1 * 6122421090493547576937037317561418841225758554253106999  *   999601352455857522148.8141657137307396757453940901242216
			// = -6119980402361616213046615863650416652406618969209180622642372279086624807992.9810268920239053350152436663869784
			//                                                                   9996013524558575221488141657137307396757453940901242216

			var matches1 = expectedResult1.Equals( result1 );
			var matches2 = expectedResult2.Equals( result2 );
			var matches3 = expectedResult3.ToString().Equals( result3.ToString().Replace( ".", "" ) );

			Assert.True( matches1 );
			Assert.True( matches2 );
			Assert.True( matches3 );
		}

		[Test]
		public void TestNegate() {
			const String expected = "-1.375";

			var result = BigDecimal.Negate( 1.375 );

			Assert.AreEqual( expected, result.ToString() );
		}

		[Test]
		public void TestReciprocal001() {
			// 1 / 3 = 0.333333333333333
			var expectedResult = BigDecimal.Parse( "0.3333333333333333" );

			var dividend = new BigDecimal( 1 );
			var divisor = new BigDecimal( 3 );

			var result = BigDecimal.Divide( dividend, divisor );

			var expected = expectedResult.ToString();
			var actual = result.ToString();

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestReciprocal002() {
			// 1/2 = 0.5
			var expectedResult = BigDecimal.Parse( "0.5" );

			var dividend = new BigDecimal( 1 );
			var divisor = new BigDecimal( 2 );

			var result = BigDecimal.Divide( dividend, divisor );

			var expected = expectedResult.ToString();
			var actual = result.ToString();

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestReciprocal003() {
			// 1/0.0833333333333333 == 12
			var expectedResult = BigDecimal.Parse( "12" );

			var dividend = new BigDecimal( 1 );
			var divisor = BigDecimal.Parse( "0.08333333333333333" );

			var result = BigDecimal.Divide( dividend, divisor );

			var expected = expectedResult.ToString();
			var actual = result.ToString();

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestReciprocal004() {
			// 2/0.63661977236758 == 3.1415926535898
			var expectedResult = BigDecimal.Parse( "3.14159265358970" );

			var dividend = new BigDecimal( 2 );
			var divisor = BigDecimal.Parse( "0.63661977236758" );

			var result = BigDecimal.Divide( dividend, divisor );

			var expected = expectedResult.ToString();
			var actual = result.ToString();

			Assert.AreEqual( expected, actual );
		}

		[Test]
		public void TestSqrt() {
			var expectedResult = BigInteger.Parse( "8145408529" );

			// sqrt(66347680104305943841) = 8145408529

			var squareNumber = BigInteger.Parse( "66347680104305943841" );
			var remainder = new BigInteger();
			var result = squareNumber.NthRoot( 2, ref remainder );

			Assert.AreEqual( expectedResult, result );
		}

		[Test]
		public void TestSubtraction() {
			var number = BigDecimal.Parse( "4294967295" );
			var expectedResult = BigDecimal.Parse( "2147483648" );

			var result = number - 2147483647;

			Assert.AreEqual( expectedResult, result );
		}

	}

}