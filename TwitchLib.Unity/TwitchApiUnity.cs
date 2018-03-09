using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Interfaces;
using TwitchLib.Api.Sections;
using UnityEngine;

namespace TwitchLib.Unity
{
    public class TwitchApiUnity : TwitchAPI, ITwitchAPI
    {
        private readonly GameObject _threadDispatcher;
		
        public TwitchApiUnity():base()
        {
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationMonoFix;

            _threadDispatcher = new GameObject("TwitchApiUnityDispatcher");
            _threadDispatcher.AddComponent<TwitchLibUnityThreadDispatcher>();
            UnityEngine.Object.DontDestroyOnLoad(_threadDispatcher);

        }

        public bool CertificateValidationMonoFix(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;

            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            foreach (X509ChainStatus status in chain.ChainStatus)
            {
                if (status.Status == X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    continue;
                }

                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;

                bool chainIsValid = chain.Build((X509Certificate2)certificate);

                if (!chainIsValid)
                {
                    isOk = false;
                }
            }

            return isOk;
        }
		
        public void Invoke<T>(Func<Task<T>> func, Action<T> action)
        {
            Task.Run(func).ContinueWith((x) =>
            {               
                var value = x.Result;
                TwitchLibUnityThreadDispatcher.Instance().Enqueue(() => action.Invoke(value));
            });
        }
    }


}
