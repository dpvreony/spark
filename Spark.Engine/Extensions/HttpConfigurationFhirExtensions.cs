﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Validation;

using System.Net.Http;
using System.Net.Http.Formatting;

using Spark.Filters;
using Spark.Handlers;
using Spark.Formatters;
using Spark.Service;

namespace Spark.Core
{ 
    public static class HttpConfigurationFhirExtensions
    {
        public static void AddFhirFormatters(this HttpConfiguration config, bool clean = true)
        {
            // remove existing formatters
            if (clean) config.Formatters.Clear();

            // Hook custom formatters            
            config.Formatters.Add(new XmlFhirFormatter());
            config.Formatters.Add(new JsonFhirFormatter());
            config.Formatters.Add(new BinaryFhirFormatter());
            config.Formatters.Add(new HtmlFhirFormatter());

            // Add these formatters in case our own throw exceptions, at least you
            // get a decent error message from the default formatters then.
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.Add(new XmlMediaTypeFormatter());
        }

        public static void AddFhirExceptionFilter(this HttpConfiguration config)
        {
            config.Filters.Add(new FhirExceptionFilter());
        }
        
        public static void AddFhirMessageHandlers(this HttpConfiguration config)
        {
            config.MessageHandlers.Add(new InterceptBodyHandler());
            config.MessageHandlers.Add(new FhirMediaTypeHandler());
            config.MessageHandlers.Add(new FhirResponseHandler());
        }
        
        public static void AddFhir(this HttpConfiguration config, params string[] endpoints)
        {
            
            config.AddFhirMessageHandlers();
            config.AddFhirExceptionFilter();
            
            // Hook custom formatters            
            config.AddFhirFormatters();

            // EK: Remove the default BodyModel validator. We don't need it,
            // and it makes the Validation framework throw a null reference exception
            // when the body empty. This only surfaces when calling a DELETE with no body,
            // while the action method has a parameter for the body.
            config.Services.Replace(typeof(IBodyModelValidator), null);
        }

    }
    
}
