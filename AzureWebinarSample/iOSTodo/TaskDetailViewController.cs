// This file has been autogenerated from parsing an Objective-C header file added in Xcode.

using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.AVFoundation;
using AzurePortable;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace iOSTodo
{
	public partial class TaskDetailViewController : UITableViewController
	{
		TodoItem currentTodoItem {get;set;}
		public RootViewController Delegate {get;set;}

		public TaskDetailViewController (IntPtr handle) : base (handle)
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			SaveButton.TouchUpInside += async (sender, e) => {
				currentTodoItem.Name = TitleText.Text;
				currentTodoItem.Notes = NotesText.Text;
				currentTodoItem.Done = DoneSwitch.On;

				//HACK: AppDelegate.Current.TaskMgr.SaveTask(currentTodoItem);
				await AppDelegate.Current.TaskMgr.SaveTaskAsync(currentTodoItem);

				// VALIDATION
//				try 
//				{
//					await AppDelegate.Current.TaskMgr.SaveTodoItemAsync(currentTodoItem);
//				} 
//				catch (MobileServiceInvalidOperationException ioe)
//				{
//					if (ioe.Response.StatusCode == System.Net.HttpStatusCode.BadRequest) {
//						// configured in portal
//						CreateAndShowDialog (ioe.Message, "Invalid");
//					} else {
//						// another error that we are not expecting
//						CreateAndShowDialog (ioe.Response.Content.ToString (), "Error");
//					}
//				}

				NavigationController.PopViewControllerAnimated(true);
			};

			DeleteButton.TouchUpInside += async (sender, e) => {
				//HACK: AppDelegate.Current.TaskMgr.DeleteTask(currentTodoItem);
				await AppDelegate.Current.TaskMgr.DeleteTaskAsync(currentTodoItem);
				NavigationController.PopViewControllerAnimated(true);
			};


			// SPEECH
			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) {
				// requires iOS 7
				SpeakButton.TouchUpInside += (sender, e) => {
					Speak (TitleText.Text + ". " + NotesText.Text);
				};
			} else {
				SpeakButton.SetTitleColor (UIColor.Gray, UIControlState.Disabled);
				SpeakButton.Enabled = false;
			}
		}

		// this will be called before the view is displayed (from the list screen)
		public void SetTask (TodoItem todo) {
			currentTodoItem = todo;
		}
		// when displaying, set-up the properties
		// expects SetTask() to be called first!
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			TitleText.Text = currentTodoItem.Name;
			NotesText.Text = currentTodoItem.Notes;
			DoneSwitch.On = currentTodoItem.Done;
		}

		float volume = 0.5f;
		float pitch = 1.0f;
		/// <summary>
		/// Speak example from: 
		/// http://blog.xamarin.com/make-your-ios-7-app-speak/
		/// </summary>
		void Speak (string text)
		{
			var speechSynthesizer = new AVSpeechSynthesizer ();

			var speechUtterance = new AVSpeechUtterance (text) {
				Rate = AVSpeechUtterance.MaximumSpeechRate/4,
				Voice = AVSpeechSynthesisVoice.FromLanguage ("en-AU"),
				Volume = volume,
				PitchMultiplier = pitch
			};

			speechSynthesizer.SpeakUtterance (speechUtterance);
		}

		void CreateAndShowDialog (string title, string error)
		{
			UIAlertView alert = new UIAlertView() { 
				Title = title, 
				Message = error
			} ;
			alert.AddButton("OK");
			alert.Show();
		}
	}
}