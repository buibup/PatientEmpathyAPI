﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(PatientEmpathy.Startup))]

namespace PatientEmpathy
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
