// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Triton.Workstation.CocoaClient
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTextField lblText { get; set; }

		[Action ("btnClick:")]
		partial void btnClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (lblText != null)
            {
				lblText.Dispose();
				lblText = null;
            }
		}
	}
}
