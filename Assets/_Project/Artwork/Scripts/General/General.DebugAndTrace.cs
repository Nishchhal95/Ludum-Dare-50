using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Provides debug and and runtime tracing tools.
/// DebugAndTrace method does logging during debug/ development and does tracing.
/// </summary>
namespace HappyFlowGames.General
{
    #region Enums
    // Types of logging.
    // Same as UnityEngine.LogType. Extended by everything behind Exception.
    public enum DnTType
    {
        Error,
        Assert,
        Warning,
        Log,
        Exception,
        Data,
        Separator
    }
    #endregion Enums

    /// <summary>
    /// Logging during debug/ development and does tracing.
    /// </summary>
    [DebuggerStepThrough]
    public class DnT

    // Note: MonoSingleton has MonoBehaviour as baseclass, therefore as well DebugAndTrace.
    // TODO Extend ILogHandler interface?
    // TODO disable if not debug build. Make it much more performant, without per cscript reporting. Simpler layout!
    // TODO add two level of log. One general and one only for runtime.
    // TODO disable on DnTTYpe level
    // TODO Maybe not necessary. Make it Singleton.
    // TODO Debuger pause in case of error, warning, assertion.
    {
        #region Enablement and Disablement

        // In order to enalbe and disable logging in general or for a period of time.
        protected static bool debugAndTraceGenerallyEnabled = true;

        // In order to enable and disable logging per c script file.
        // Hash list of the calling c script file names.
        private static List<int> callingCScriptHashList = new List<int>();
        // Note: elements are added at end of list (standard list.add method behavior), therefore both lists are in sync.
        private static List<bool> callingCScriptDebugAndTraceSwitchList = new List<bool>();

        /// <summary>
        /// Allows to enable and disable DebugAndTrace per c script file.
        /// </summary>
        /// <param name="doDebugAndTrace">If set to <c>true</c> do debug and trace.</param>
        [DebuggerHidden]
        public static void EnableDisablePerCScript(bool doDebugAndTrace)
        {
            int cScriptListIndex;

            callingCScriptCheckHasDebugAndTraceSwitchEnabled(1, out cScriptListIndex);
            callingCScriptDebugAndTraceSwitchList[cScriptListIndex] = doDebugAndTrace;
        }

        /// <summary>
        /// Enable and disable debug and trace in general. Or temporarily by en/disabling and reen/redisabling it.
        /// </summary>
        /// <param name="doDebugAndTrace">If set to <c>true</c> then debug and trace is switched off.</param>
        [DebuggerHidden]
        public static void EnableDisableInGeneral(bool doDebugAndTrace)
        {
            debugAndTraceGenerallyEnabled = doDebugAndTrace;
        }

        #endregion Enablement and Disablement

        #region Internal helper methods

        /// <summary>
        /// Formats the output in a general way.
        /// </summary>
        private static string formatOutput(DnTType dntType, System.Object context, string location, string messageAndDetails, params object[] detailsArguments)
        {
            // In order to always have an "!" in an important output, even if message string does not exist.
            // For less important output "." is sufficient.
            string messageAndDetailsPunctuation;
            switch (dntType)
            {
                case DnTType.Error:
                case DnTType.Assert:
                case DnTType.Warning:
                case DnTType.Exception:
                    messageAndDetailsPunctuation = "!";
                    break;
                case DnTType.Log:
                case DnTType.Data:
                    if (!messageAndDetails.EndsWith(Environment.NewLine))
                        messageAndDetailsPunctuation = ".";
                    else
                        messageAndDetailsPunctuation = string.Empty;
                    break;
                case DnTType.Separator:
                default:
                    messageAndDetailsPunctuation = string.Empty;
                    break;
            }

            string contextString;
            if (context == null || (context is float && (float)context == 0f) || (context is int && (int)context == 0))
            {
                contextString = string.Empty;
            }
            else
            {
                contextString = string.Concat(" Context: ", context.ToString(), ".");
            }

            string locationString;
            if (location == string.Empty || location == null)
            {
                locationString = string.Empty;
            }
            else
            {
                locationString = string.Concat(location, ": ");
            }

            string messageAndDetailsString;
            if (messageAndDetails == string.Empty || messageAndDetails == null)
            {
                messageAndDetailsString = string.Empty;
            }
            else
            {
                messageAndDetailsString = string.Format(messageAndDetails + messageAndDetailsPunctuation, detailsArguments);
            }

            return (string.Format("{0}{1}{2}", locationString, messageAndDetailsString, contextString));
        }

        /// <summary>
        /// In order to enable and disable per c script file.
        /// Checks if the calling c script already has used DebugAndTrace, if not then it is added to lists.
        /// 
        /// </summary>
        /// <returns>Returns <c>true</c> if the calling c script has DebugAndTrace enabled, <c>false</c> otherwise.</returns>
        /// <param name="framesInbetween">Stack frames inbetween, i.e. in this c script file after the original calll.</param>
        /// <param name="out callingFileIndex">Calling c script file index in the lists.</param>
        private static bool callingCScriptCheckHasDebugAndTraceSwitchEnabled(int framesInbetween, out int callingFileIndex)
        {
            // In order to be able to disable messages per .cs file, get the calling stack.
            // callingCScriptCheckHasDebugAndTraceSwitchEnabled (stack frame 0) was called by communicate or EnableDisable (stack frame 1).
            // Communicate was called by one of the other methods below (stack frame 2), which was called by the logging method (stack frame 3).
            // EnableDisable was called directly (stack frame 2).
            // Only works in debug versions!
            // TODO Disable for runtime

            string callingFileName = new System.Diagnostics.StackTrace(true).GetFrame(framesInbetween + 1).GetFileName();
            int callingFileNameHash = callingFileName.GetHashCode();
            callingFileIndex = callingCScriptHashList.IndexOf(callingFileNameHash);

            // If calling cs file already used DebugAndTrace.
            if (callingFileIndex != -1)
            {
                // If its switch is set to false, the don't do anything.
                return (callingCScriptDebugAndTraceSwitchList[callingFileIndex]);
            }
            else
            {
                // Calling cs file uses DebugAndTrace for first time.
                // As we add after assignment of count, the value is the index of the added one.
                callingFileIndex = callingCScriptHashList.Count;
                callingCScriptHashList.Add(callingFileNameHash);
                callingCScriptDebugAndTraceSwitchList.Add(true);
                // For a new c script assumption is that it wants to be logged.
                return (true);
            }
        }

        private static bool callingCScriptCheckHasDebugAndTraceSwitchEnabled(int framesInbetween)
        {
            int throwAwayIndex;
            // Adding one to framesInbetween, as its calling "itself".
            return (callingCScriptCheckHasDebugAndTraceSwitchEnabled(framesInbetween + 1, out throwAwayIndex));
        }

        /// <summary>
        /// Different log types use different underlying log methods. These are called here.
        /// </summary>
        private static void communicate(DnTType dntType, System.Object context, string location, string messageAndDetails, params object[] detailsArguments)
        {
            // No debug and trace if the calling script has disabled it.
            if (!callingCScriptCheckHasDebugAndTraceSwitchEnabled(2))
                return;

            // TODO Add enabling, disabling DnTType dependent.

            // Note: the new line at the end of the reporting is to "hide" "UnityEngine.Debug:Log(object)" in the standard unity editor console.
            switch (dntType)
            {
                case DnTType.Error:
                    UnityEngine.Debug.LogError("Error: " + formatOutput(dntType, (System.Object)context, location, messageAndDetails, detailsArguments) + Environment.NewLine);
                    break;
                case DnTType.Assert:
                    UnityEngine.Debug.LogWarning("Assertion false: " + formatOutput(dntType, (System.Object)context, location, messageAndDetails, detailsArguments) + Environment.NewLine);
                    break;
                case DnTType.Warning:
                    UnityEngine.Debug.LogWarning("Warning: " + formatOutput(dntType, (System.Object)context, location, messageAndDetails, detailsArguments) + Environment.NewLine);
                    break;
                case DnTType.Log:
                    UnityEngine.Debug.Log(formatOutput(dntType, (System.Object)context, location, messageAndDetails, detailsArguments) + Environment.NewLine);
                    break;
                case DnTType.Exception:
                    UnityEngine.Debug.LogError("Exception: Unknown exception type. " + formatOutput(dntType, (System.Object)context, location, messageAndDetails, detailsArguments) + Environment.NewLine);
                    break;
                case DnTType.Data:
                    string preContextString;
                    if (messageAndDetails.EndsWith(Environment.NewLine))
                        preContextString = string.Empty;
                    else
                        preContextString = " ";
                    UnityEngine.Debug.Log("Data: " + formatOutput(dntType, null, location, messageAndDetails, detailsArguments) + preContextString + (context ?? "<null>").ToString() + "." + Environment.NewLine);
                    break;
                case DnTType.Separator:
                    UnityEngine.Debug.Log("======================================================" + Environment.NewLine);
                    break;
                default:
                    UnityEngine.Debug.Log("---");
                    break;
            }
        }

        /// <summary>
        /// An exception should as well show information about the actual exception, therfore extra method.
        /// </summary>

        private static void communicate(Exception exception, DnTType dntType, System.Object context, string location, string messageAndDetails, params object[] detailsArguments)
        {
            // No debug and trace if the calling script has disabled it.
            if (!callingCScriptCheckHasDebugAndTraceSwitchEnabled(2))
                return;

            UnityEngine.Debug.LogError("Exception: More information in next log entry. " + formatOutput(dntType, (System.Object)context, location, messageAndDetails, detailsArguments));
            if (context.GetType() == typeof(UnityEngine.Object))
            {
                UnityEngine.Debug.LogException(exception, (UnityEngine.Object)context);
            }
            else
            {
                UnityEngine.Debug.Log("Exception context: " + context + ". " + context.ToString() + Environment.NewLine);
                UnityEngine.Debug.LogException(exception);
            }
        }

        /// <summary>
        /// Show a visual separator.
        /// </summary>
        /// <param name="length">Length.</param>
        private static void communicate(DnTType dntType, int length)
        {
            // No debug and trace if the calling script has disabled it.
            if (!callingCScriptCheckHasDebugAndTraceSwitchEnabled(2))
                return;

            string separatorString;

            switch (length)
            {
                case 0:
                    separatorString = "==================================================================================================================================================================";
                    break;
                case 1:
                    separatorString = "============================================================================================================";
                    break;
                case 2:
                    separatorString = "======================================================";
                    break;
                case 3:
                    separatorString = "==================";
                    break;
                case 4:
                    separatorString = "=========";
                    break;
                case 5:
                    separatorString = "===";
                    break;
                default:
                    separatorString = "===";
                    break;
            }

            UnityEngine.Debug.Log(separatorString + Environment.NewLine);
        }

        /// <summary>
        /// Gets the timestamp as a string.
        /// </summary>
        /// <returns>The timestamp as a string.</returns>
        private static string getTimestamp()
        {
            return System.DateTime.UtcNow.ToString("HH:mm:ss.fff");
        }

        #endregion Internal helper methods

        #region Exception reporting

        /// <summary>
        /// Exception reporting for debugging and to trace at runtime.
        /// </summary>
        /// <param name="exception">Exception information.</param>
        /// <param name="context">Context.</param>
        /// <param name="location">Location.</param>
        /// <param name="message">Message.</param>
        /// <param name="detailsFormat">Detailed information format.</param>
        /// <param name="detailsArguments">Actual detailed information.</param>
        [DebuggerHidden]
        public static void Exception(Exception exception, System.Object context, string location, string messageAndDetails, params object[] detailsArguments)
        {
            if (debugAndTraceGenerallyEnabled)
                communicate(exception, DnTType.Exception, context, location, messageAndDetails, exception);
        }

        #endregion Exception reporting

        #region Error reporting

        /// <summary>
        /// Report errors for debugging and to trace at runtime.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="location">Location.</param>
        /// <param name="message">Message.</param>
        /// <param name="detailsFormat">Detailed information format.</param>
        /// <param name="detailsArguments">Actual detailed information.</param>
        [DebuggerHidden]
        public static void Error(System.Object context, string location, string messageAndDetails, params object[] detailsArguments)
        {
            if (debugAndTraceGenerallyEnabled)
                communicate(DnTType.Error, context, location, messageAndDetails, detailsArguments);
        }

        #endregion Error reporting

        #region Warning reporting

        /// <summary>
        /// Report warnings for debugging and to trace at runtime.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="location">Location.</param>
        /// <param name="message">Message.</param>
        /// <param name="detailsFormat">Detailed information format.</param>
        /// <param name="detailsArguments">Actual detailed information.</param>
        [DebuggerHidden]
        public static void Warning(System.Object context, string location, string messageAndDetails, params object[] detailsArguments)
        {
            if (debugAndTraceGenerallyEnabled)
                communicate(DnTType.Warning, context, location, messageAndDetails, detailsArguments);
        }

        /// <summary>
        /// Report warnings for debugging and to trace at runtime.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="message">Message.</param>
        [DebuggerHidden]
        public static void Warning(string location, string message)
        {
            if (debugAndTraceGenerallyEnabled)
                communicate(DnTType.Warning, null, location, message, string.Empty, null);
        }

        #endregion Warning reporting

        #region Assertion reporting

        /// <summary>
        /// Checks the assertion.
        /// If not correct, then output with information.
        /// </summary>
        /// <param name="assertion">Assertion in form of a bool or comparison or ...</param>
        /// <param name="location">Location.</param>
        /// <param name="messageAndDetails">Message and details.</param>
        /// <param name="detailsArguments">Details arguments.</param>
        /// <returns>Returns the assertion, so Assert can be called in an if condition.</returns>
        [DebuggerHidden]
        public static bool Assert(bool assertion, string location, string messageAndDetails, params object[] detailsArguments)
        {
            return (Assert(false, assertion, location, messageAndDetails, detailsArguments));
        }

        [DebuggerHidden]
        public static bool Assert(bool silent, bool assertion, string location, string messageAndDetails, params object[] detailsArguments)
        {
            if (debugAndTraceGenerallyEnabled)
                if (!silent && !assertion)
                    communicate(DnTType.Assert, null, location, messageAndDetails, detailsArguments);

            return (assertion);
        }

        #endregion Assertion reporting

        #region Informational reporting

        /// <summary>
        /// Report information for debugging and to trace at runtime.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="location">Location.</param>
        /// <param name="message">Message.</param>
        [DebuggerHidden]
        public static void Log(System.Object context, string location, string messageAndDetails, params object[] detailsArguments)
        {
            if (debugAndTraceGenerallyEnabled)
                communicate(DnTType.Log, context, location, messageAndDetails, detailsArguments);
        }

        /// <summary>
        /// Report information for debugging and to trace at runtime.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="message">Message.</param>
        /// <param name="detailsFormat">Detailed information format.</param>
        /// <param name="detailsArguments">Actual detailed information.</param>
        [DebuggerHidden]
        public static void Log(string location, string messageAndDetails, params object[] detailsArguments)
        {
            if (debugAndTraceGenerallyEnabled)
                communicate(DnTType.Log, null, location, messageAndDetails, detailsArguments);
        }

        /// <summary>
        /// Show a string
        /// </summary>
        /// <param name="text"></param>
        [DebuggerHidden]
        public static void Log(string text)
        {
            if (debugAndTraceGenerallyEnabled)
                communicate(DnTType.Log, null, string.Empty, text);
        }
        #endregion Informational reporting

        #region Data reporting

        /// <summary>
        /// Report application data for debugging and to trace at runtime.
        /// </summary>
        /// <param name="theDataObject">The data object.</param>
        /// <param name="location">Location.</param>
        /// <param name="message">Message.</param>
        /// <param name="detailsFormat">Detailed information format.</param>
        /// <param name="detailsArguments">Actual detailed information.</param>
        [DebuggerHidden]
        public static void Data(System.Object theDataObject, string location, string messageAndDetails, params object[] detailsArguments)
        {
            if (debugAndTraceGenerallyEnabled)
                communicate(DnTType.Data, theDataObject, location, messageAndDetails, detailsArguments);
        }

        /// <summary>
        /// Report application data for debugging and to trace at runtime.
        /// </summary>
        /// <param name="theDataObject">The data object.</param>
        [DebuggerHidden]
        public static void Data(System.Object theDataObject)
        {
            if (debugAndTraceGenerallyEnabled)
                communicate(DnTType.Data, theDataObject, string.Empty, string.Empty, string.Empty, null);
        }

        #endregion Data reporting

        #region Visual reporting

        /// <summary>
        /// Pure visual separator for debugging and tracing.
        /// </summary>
        /// <param name="lenght">Lenght.</param>
        [DebuggerHidden]
        public static void Separator(int length)
        {
            if (debugAndTraceGenerallyEnabled)
                communicate(DnTType.Separator, length);
        }

        #endregion Visual reporting

        #region Time reporting
        /// <summary>
        /// Time Stamp
        /// </summary>
        [DebuggerHidden]
        public static void TimeStamp()
        {
            if (debugAndTraceGenerallyEnabled)
                communicate(DnTType.Log, 0f, "", "{0}", getTimestamp());
        }
        /// <summary>
        /// Time stamp with note
        /// </summary>
        /// <param name="note"></param>
        [DebuggerHidden]
        public static void TimeStamp(string note)
        {
            if (debugAndTraceGenerallyEnabled)
                communicate(DnTType.Log, 0f, note, "{0}", getTimestamp());
        }

        #endregion Time reporting
    }
}