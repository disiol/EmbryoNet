using System;
using Firebase.Crashlytics;
using UnityEngine;

// Import Firebase and Crashlytics

namespace Tolls
{
    public class CrashlyticsInit : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            // Initialize Firebase
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    // Crashlytics will use the DefaultInstance, as well;
                    // this ensures that Crashlytics is initialized.
                    Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;

                    // When this property is set to true, Crashlytics will report all
                    // uncaught exceptions as fatal events. This is the recommended behavior.
                    Crashlytics.ReportUncaughtExceptionsAsFatal = true;

                    // Set a flag here for indicating that your project is ready to use Firebase.

                    // try
                    // {
                    //     throwExceptionEvery60Updates();
                    // }
                    // catch (Exception e)
                    // {
                    //     Crashlytics.LogException(e);
                    //
                    //     Console.WriteLine(e);
                    // }

                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                        "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }

      

        // A method that tests your Crashlytics implementation by throwing an
        // exception every 60 frame updates. You should see reports in the
        // Firebase console a few minutes after running your app with this method.
        void throwExceptionEvery60Updates()
        {
            // Throw an exception to test your Crashlytics implementation
            throw new System.Exception("test exception please ignore");
        }
    }
}