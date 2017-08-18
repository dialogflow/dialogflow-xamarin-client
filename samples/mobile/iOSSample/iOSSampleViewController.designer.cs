/**
 * Copyright 2017 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace iOSSample
{
	[Register ("iOSSampleViewController")]
	partial class iOSSampleViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton cancelButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton listenButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView resultTextView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton selectLang { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIProgressView soundLevelView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton stopButton { get; set; }

		[Action ("cancelButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void cancelButton_TouchUpInside (UIButton sender);

		[Action ("listenButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void listenButton_TouchUpInside (UIButton sender);

		[Action ("selectLang_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void selectLang_TouchUpInside (UIButton sender);

		[Action ("stopButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void stopButton_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (cancelButton != null) {
				cancelButton.Dispose ();
				cancelButton = null;
			}
			if (listenButton != null) {
				listenButton.Dispose ();
				listenButton = null;
			}
			if (resultTextView != null) {
				resultTextView.Dispose ();
				resultTextView = null;
			}
			if (selectLang != null) {
				selectLang.Dispose ();
				selectLang = null;
			}
			if (soundLevelView != null) {
				soundLevelView.Dispose ();
				soundLevelView = null;
			}
			if (stopButton != null) {
				stopButton.Dispose ();
				stopButton = null;
			}
		}
	}
}
