/*
 * Project: TwitchToolkit
 * File: WebRequest_BeginGetResponse.cs
 * 
 * Usage: Provides asynchronous web request handling with callbacks and error management.
 * Deprecated: TwitchLib 3.4.0 and later have built-in async support; consider using HttpClient for new implementations.
 */
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using TwitchToolkit;

namespace TwitchToolkitDev;

public class WebRequest_BeginGetResponse
{
	public static ManualResetEvent allDone = new ManualResetEvent(initialState: false);

	private const int BUFFER_SIZE = 1024;

	public static void Main(string requesturl, Func<RequestState, bool> func = null)
	{
		ToolkitLogger.Log("This is a deprecated test class. Use HttpClient or TwitchLib 3.4.0+ for async web requests.");
		return;
        // We KNow this is deprecated but leaving it here for reference
        ToolkitLogger.Log(requesturl);
		try
		{
			ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
			WebRequest myWebRequest = WebRequest.Create(requesturl);
			RequestState myRequestState = new RequestState();
			myRequestState.urlCalled = requesturl;
			myRequestState.Callback = func;
			myRequestState.request = myWebRequest;
			IAsyncResult asyncResult = myWebRequest.BeginGetResponse(RespCallback, myRequestState);
			allDone.WaitOne();
			Console.Read();
		}
		catch (WebException e2)
		{
            ToolkitLogger.Log("WebException raised! Main");
			ToolkitLogger.Log("\n" + e2.Message);
			ToolkitLogger.Log($"\n{e2.Status}");
		}
		catch (Exception e)
		{
			ToolkitLogger.Log("Exception raised! - get");
            ToolkitLogger.Log("Source : " + e.Source);
            ToolkitLogger.Log("Message : " + e.Message + " " + e.StackTrace);
		}
	}

	public static void Delete(string requesturl, Func<RequestState, bool> func = null, WebHeaderCollection headers = null)
	{
		try
		{
			ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
			WebRequest myWebRequest = WebRequest.Create(requesturl);
			myWebRequest.Method = "DELETE";
			myWebRequest.Headers = headers;
			RequestState myRequestState = new RequestState();
			myRequestState.urlCalled = requesturl;
			myRequestState.Callback = func;
			myRequestState.request = myWebRequest;
			IAsyncResult asyncResult = myWebRequest.BeginGetResponse(RespCallback, myRequestState);
			allDone.WaitOne();
			Console.Read();
		}
		catch (WebException e2)
		{
            ToolkitLogger.Log("WebException raised! Delete");
            ToolkitLogger.Log("\n" + e2.Message);
            ToolkitLogger.Log($"\n{e2.Status}");
		}
		catch (Exception e)
		{
            ToolkitLogger.Log("Exception raised! - get");
            ToolkitLogger.Log("Source : " + e.Source);
            ToolkitLogger.Log("Message : " + e.Message + " " + e.StackTrace);
		}
	}

	public static void Put(string requesturl, Func<RequestState, bool> func = null, WebHeaderCollection headers = null)
	{
		try
		{
			ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
			WebRequest myWebRequest = WebRequest.Create(requesturl);
			myWebRequest.Method = "PUT";
			myWebRequest.Headers = headers;
			RequestState myRequestState = new RequestState();
			myRequestState.urlCalled = requesturl;
			myRequestState.Callback = func;
			myRequestState.request = myWebRequest;
			IAsyncResult asyncResult = myWebRequest.BeginGetResponse(RespCallback, myRequestState);
			allDone.WaitOne();
			Console.Read();
		}
		catch (WebException e2)
		{
            ToolkitLogger.Log("WebException raised!");
            ToolkitLogger.Log("\n" + e2.Message);
            ToolkitLogger.Log($"\n{e2.Status}");
		}
		catch (Exception e)
		{
            ToolkitLogger.Log("Exception raised! - get");
            ToolkitLogger.Log("Source : " + e.Source);
            ToolkitLogger.Log("Message : " + e.Message + " " + e.StackTrace);
		}
	}

	private static void RespCallback(IAsyncResult asynchronousResult)
	{
		// We won't signal that we're done with the HTTP request within the
		// "try" block as we still need to *read* the contents of the response,
		// but we'll signal that the request is done should an error be raised
		// at any point during the process.
		
		try
		{
			RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
			WebRequest myWebRequest1 = myRequestState.request;
			myRequestState.response = myWebRequest1.EndGetResponse(asynchronousResult);
			IAsyncResult asynchronousResultRead = (myRequestState.responseStream = myRequestState.response.GetResponseStream()).BeginRead(myRequestState.bufferRead, 0, 1024, ReadCallBack, myRequestState);
			myRequestState.response.Close();
		}
		catch (WebException e2)
		{
            ToolkitLogger.Log("WebException raised - callback! RespCallback");
            ToolkitLogger.Log("\n" + e2.Message);
            ToolkitLogger.Log($"\n{e2.Status}");

			allDone.Set();
		}
		catch (Exception e)
		{
            ToolkitLogger.Log("Exception raised!");
			ToolkitLogger.Log("Source : " + e.Source);
			ToolkitLogger.Log("Message : " + e.Message + " " + e.StackTrace);

			allDone.Set();
		}
	}

	private static void ReadCallBack(IAsyncResult asyncResult)
	{
		// We'll signal that the request is done when 
		
		try
		{
			RequestState myRequestState = (RequestState)asyncResult.AsyncState;
			Stream responseStream = myRequestState.responseStream;
			int read = responseStream.EndRead(asyncResult);
			if (read > 0)
			{
				myRequestState.requestData.Append(Encoding.ASCII.GetString(myRequestState.bufferRead, 0, read));
				IAsyncResult asynchronousResult = responseStream.BeginRead(myRequestState.bufferRead, 0, 1024, ReadCallBack, myRequestState);
				return;
			}
			if (myRequestState.requestData.Length > 1)
			{
				ToolkitLogger.Log(myRequestState.jsonString = myRequestState.requestData.ToString());
				if (myRequestState.Callback != null)
				{
					myRequestState.Callback(myRequestState);
				}
			}
			responseStream.Close();
		}
		catch (WebException e2)
		{
			ToolkitLogger.Log("WebException raised - read!");
			ToolkitLogger.Log("\n" + e2.Message);
			ToolkitLogger.Log($"\n{e2.Status}");
		}
		catch (Exception e)
		{
			ToolkitLogger.Log("Exception raised!");
			ToolkitLogger.Log("Source : " + e.Source);
			ToolkitLogger.Log("Message : " + e.Message + " " + e.StackTrace);
		}

		allDone.Set();
	}

	public static bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
		bool isOk = true;
		if (sslPolicyErrors != 0)
		{
			for (int i = 0; i < chain.ChainStatus.Length; i++)
			{
				if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
				{
					chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
					chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
					chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
					chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
					if (!chain.Build((X509Certificate2)certificate))
					{
						isOk = false;
						break;
					}
				}
			}
		}
		return isOk;
	}
}
