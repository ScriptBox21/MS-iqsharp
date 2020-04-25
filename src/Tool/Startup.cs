﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Quantum.Canon;
using Microsoft.Quantum.Chemistry.Magic;
using Microsoft.Quantum.IQSharp.Jupyter;
using Microsoft.Quantum.Simulation.Core;
using System;

namespace Microsoft.Quantum.IQSharp
{
    /// <summary>
    /// StartUp class used when starting as a WebHost (http server)
    /// </summary>
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Workspace.Settings>(Program.Configuration);
            services.Configure<NugetPackages.Settings>(Program.Configuration);
            services.Configure<ClientInformation>(Program.Configuration);

            services.AddSingleton(typeof(ITelemetryService), GetTelemetryServiceType());
            services.AddIQSharp();
            services.AddIQSharpKernel();

            //services.AddSingleton<IReferences, References>(provider =>
            //{
            //    var refs = ActivatorUtilities.CreateInstance<References>(provider);
            //    refs.AddAssemblies(
            //        new AssemblyInfo(typeof(ApplyToEach<>).Assembly),
            //        new AssemblyInfo(typeof(Katas.KataMagic).Assembly),
            //        new AssemblyInfo(typeof(BroombridgeMagic).Assembly)
            //    );

            //    return refs;
            //});

            var assembly = typeof(PackagesController).Assembly;
            services.AddControllers()
                .AddApplicationPart(assembly);
        }

        private Type GetTelemetryServiceType()
        {
            return
#if TELEMETRY
                Program.TelemetryOptOut ? typeof(NullTelemetryService) : typeof(TelemetryService);
#else
                typeof(NullTelemetryService);
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
