﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries,
// repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper licenses and/or copyrights.
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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "CodeEngine.cs" last formatted on 2022-02-06 at 6:00 AM by Protiguous.


namespace Librainian.Extensions;

using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using Logging;
using Microsoft.CSharp;

/// <summary>TODO this engine needs be revisted.</summary>
public class CodeEngine {

	private CompilerResults? _compilerResults;

	private String? _sourceCode = String.Empty;

	public CodeEngine( String sourcePath, Action<String>? output ) : this( Guid.NewGuid(), sourcePath, output ) {
	}

	public CodeEngine( Guid id, String sourcePath, Action<String?>? output ) {
		this.Output = output;

		//if ( ID.Equals( Guid.Empty ) ) { throw new InvalidOperationException( "Null guid given" ); }
		this.SourcePath = Path.Combine( sourcePath, id + ".cs" );

		if ( !this.Load() ) {
			this.SourceCode = DefaultCode();
		}
	}

	public interface IOutput {

		void Output();
	}

	private Object CompileLock { get; } = new();

	private Object SourceCodeLock { get; } = new();

	public static CSharpCodeProvider CSharpCodeProvider { get; } = new();

	public Guid ID { get; set; }

	public Action<String>? Output { get; }

	public Object[]? Parameters { get; set; }

	public String? SourceCode {
		get {
			lock ( this.SourceCodeLock ) {
				return this._sourceCode;
			}
		}

		set {
			lock ( this.SourceCodeLock ) {
				this._sourceCode = value;
			}

			this.Compile(); //TODO schedule a task to run Compile?
		}
	}

	public String? SourcePath { get; }

	private static String DefaultCode() =>
		@"
using System;
using Libranian;

namespace Coding
{
    public class CodeEngine
    {
        private Action<String> Output = delegate { };

        public object DynamicCode(params object[] Parameters)
        {
            Output(""Hello from dynamic code!"");
            return 0;
        }
    }
}";

	/// <summary>Prepare the assembly for Run()</summary>
	private Boolean Compile() {
		try {
			CompilerResults? results;

			lock ( this.CompileLock ) {
				this._compilerResults = CSharpCodeProvider.CompileAssemblyFromSource( new CompilerParameters {
					GenerateInMemory = true,
					GenerateExecutable = false
				}, this.SourceCode );
				results = this._compilerResults;
			}

			if ( results == null ) {
				return false;
			}

			if ( results.Errors?.HasErrors == true ) {
				"Errors".Break();

				return false;
			}

			if ( results.Errors?.HasWarnings == true ) {
				return true;
			}

			"".Break();

			return true;
		}
		catch ( Exception exception ) {
			exception.Log();

			return false;
		}
	}

	public static Boolean Test( Action<String>? output ) {
		try {
			var test = new CodeEngine( Guid.Empty, Path.GetTempPath(), output );
			test.Run();

			return true;
		}
		catch ( Exception exception ) {
			exception.Log();

			return false;
		}
	}

	public Boolean Load() => String.IsNullOrEmpty( this.SourceCode );

	public Object? Run() {
		lock ( this.CompileLock ) {
			if ( this._compilerResults == null ) {
				this.Compile();
			}

			if ( this._compilerResults == null ) {
				return default( Object );
			}

			if ( this._compilerResults.Errors?.HasErrors == true ) {
				"".Break();

				return default( Object );
			}

			if ( this._compilerResults.Errors?.HasWarnings == true ) {
				"".Break();
			}

			var loAssembly = this._compilerResults.CompiledAssembly;
			var loObject = loAssembly?.CreateInstance( "Coding.CodeEngine" );

			if ( loObject is null ) {
				"".Break();

				return default( Object );
			}

			try {
				var loResult = loObject.GetType().InvokeMember( "DynamicCode", BindingFlags.InvokeMethod, null, loObject, this.Parameters );

				return loResult;
			}
			catch ( Exception exception ) {
				exception.Log();

				return default( Object );
			}
		}
	}
}