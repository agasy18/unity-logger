using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;



public static class Logger
{
	
	public enum LogType: int
	{
		Info, // log as unity info
		Warning, // log as unity warning
		Assert, //log as unity assertion and break on debug or throw AssertionException
		Error //log as error and throw Excaption
	}


	static Logger () {
		#if !DEBUG
		MinLogLevel = LogType.Warning;
		ThrowOnAssert = false;
		#endif
	}

	/// <summary>
	/// The minimum log level.
	/// </summary>
	public static LogType MinLogLevel = LogType.Info;

	/// <summary>
	/// throw on error.
	/// </summary>
	public static bool ThrowOnError = true;

	public static bool ThrowOnAssert = true;

	/// <summary>
	/// Log the specified obj.
	/// </summary>
	/// <param name="obj">Object.</param>
	/// <param name="type">Log type.</param>
	/// <typeparam name="T">The same object (this)</typeparam>
	public static T Log<T> (this T obj, LogType type = LogType.Info)
	{
		if (type<MinLogLevel) {
			return obj;
		}
		var logObj = Equals (default(T), obj) ? "null" : obj.ToString ();
		DebugWriteLine (type + " | " + logObj, type);
		return obj;
	}

	/// <summary>
	/// Log the specified obj and other objects.
	/// You can pass LogType as other object
	/// </summary>
	/// <param name="obj">Object.</param>
	/// <param name="parameters">other objects</param>
	/// <typeparam name="T">The same object (this)</typeparam>
	public static T Log<T> (this T obj, params object[] parameters)
	{
		
		StringBuilder str = new StringBuilder ();
		LogType type = LogType.Info;
		foreach (var param in parameters) {
			if (param is LogType) {
				type = (LogType)param;
			} else {
				str.Append ((param ?? "null").ToString () + ",");
			}
		}
		if (type<MinLogLevel) {
			return obj;
		}
		str.Remove (str.Length - 1, 1);
		str.Append (" = " + (obj == null ? "null" : obj.ToString ()));
		DebugWriteLine (type + " | " + str.ToString (), type);
		return obj;
	}


	/// <summary>
	/// Assert program if conditon is false.
	/// Brakes the program if debug otherwise throws the AssertionException
	/// </summary>
	/// <param name="t">condition</param>
	/// <param name="message">User message</param>
	/// <typeparam name="T">The same bool (conditon)</typeparam>
	public static bool Assert (this bool t, string message = "Condition is false")
	{
		if (default(bool) != t)
			return t;
		new Exception (message).DebugDesc().Log(LogType.Assert);
		return t;
	}


	/// <summary>
	/// Assert program if e is not null.
	/// Brakes the program if debug otherwise throws the AssertionException
	/// </summary>
	/// <param name="e">exception</param>
	/// <param name="message">User message</param>
	public static void Assert (this Exception e, string message = "Excaption")
	{
		if (e != null) {
			e.DebugDesc().Log(message, LogType.Assert);
		}
	}

	/// <summary>
	/// Assert program if object is null.
	/// Brakes the program if debug otherwise throws the AssertionException
	/// </summary>
	/// <param name="t">condition</param>
	/// <param name="message">User message</param>
	/// <typeparam name="T">The same object (this)</typeparam>
	public static T Assert<T> (this T t, string message = "Object is null") where T : class
	{
		if (t == null) {
			new Exception (message).DebugDesc().Log(LogType.Assert);
		}
		return t;
	}


	/// <summary>
	/// return deep description for some objects
	/// </summary>
	/// <returns>The description</returns>
	/// <param name="obj">Object.</param>
	/// <param name="showTypeInfo">If set to <c>true</c> to show type info.</param>
	public static string DebugDesc (this object obj, bool showTypeInfo = true)
	{
		try {
			if (obj == null)
				return "null";
			StringBuilder str = new StringBuilder ();
			if (showTypeInfo)
				str.AppendLine ("Type : " + obj.GetType ().ToString ());
			else if (obj is Exception) {
				var o = obj as Exception;
				str.AppendLine ("Message = " + o.Message);
				str.AppendLine ("InnerException =\t" + o.InnerException.DebugDesc ());
				str.AppendLine ("HelpLink = " + o.HelpLink);
				str.AppendLine ("Data =\t" + o.Data.DebugDesc ());
				str.AppendLine ("Source = " + o.Source);
				str.AppendLine ("Source = " + o.Source);
				str.AppendLine ("StackTrace = " + o.StackTrace);
			} else if (obj is IDictionary) {
				var o = obj as IDictionary;
				if (o.Count == 0) {
					str.Append ("{}");
				} else {
					str.AppendLine ("{\t");
					foreach (DictionaryEntry dictionaryEntry in o) {
						str.AppendLine ("\t" + dictionaryEntry.Key + " : " + dictionaryEntry.Value);
					}
					str.AppendLine ("}");
				}
			} else if (obj is ICollection) {

				var o = obj as ICollection;
				if (o.Count == 0) {
					str.Append ("[]");
				} else {
					str.Append ("[");
					foreach (var item in o) {
						str.Append ("\t" + item);
						str.AppendLine (",");
					}
					str.AppendLine ("]");
				}
			} else {
				obj.ToString ();
			}
			return str.ToString ();
		} catch {
			return obj.ToString ();
		}
	}


	private static void DebugWriteLine (string str, LogType type)
	{
		switch (type) {
		case LogType.Error:
			UnityEngine.Debug.LogError (str);
			if(ThrowOnError) {
				throw new ErrorException (str);
			}
			break;
		case LogType.Warning:
			UnityEngine.Debug.LogWarning (str);
			break;
		case LogType.Assert:
			UnityEngine.Debug.LogAssertion (str);
			if (System.Diagnostics.Debugger.IsAttached) {
				System.Diagnostics.Debugger.Break ();
			} else if(ThrowOnAssert) {
				throw new AssertionException (str);
			}
			break;
		case LogType.Info:
			UnityEngine.Debug.Log (str);
			break;
		}
	}

	public class ErrorException : System.Exception
	{
		public ErrorException (string message) : base (message) { }
	}

	public class AssertionException : System.Exception
	{
		public AssertionException (string message) : base (message) { }
	}
}