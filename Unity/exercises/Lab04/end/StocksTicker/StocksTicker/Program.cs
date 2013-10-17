//===============================================================================
// Microsoft patterns & practices
// Enterprise Library 6 and Unity 3 Hands-on Labs
//===============================================================================
// Copyright � Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
using System;
using System.Windows.Forms;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

using PersistenceFramework;
using StocksTicker.Loggers;
using StocksTicker.StockQuoteServices;
using StocksTicker.UI;

namespace StocksTicker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (IUnityContainer container = new UnityContainer())
            {
                container.LoadConfiguration();

                container
                    .RegisterType(
                        typeof(IRepository<>),
                        typeof(ValidatingRepository<>),
                        "validating")
                    .RegisterType<IValidator<StockQuote>, RandomStockQuoteValidator>()
                    .RegisterType<ILogger, CompositeLogger>(
                        "composite",
                        new InjectionConstructor(
                            new ResolvedArrayParameter<ILogger>(
                                typeof(ILogger),
                                new ResolvedParameter<ILogger>("UI"))));

                StocksTickerPresenter presenter
                    = container.Resolve<StocksTickerPresenter>(
                        new ParameterOverride(
                            "repository",
                            new ResolvedParameter<IRepository<StockQuote>>("validating")).OnType<StocksTickerPresenter>(),
                        new PropertyOverride("Logger", new ResolvedParameter<ILogger>("composite")));

                Application.Run((Form)presenter.View);
            }
        }
    }
}