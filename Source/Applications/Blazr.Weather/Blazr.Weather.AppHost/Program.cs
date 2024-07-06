var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Blazr_App_Fluent_Server>("blazr-app-fluent-server");

builder.AddProject<Projects.Blazr_App_Fluent_WASM_Server>("blazr-app-fluent-wasm-server");

builder.AddProject<Projects.Blazr_App_MudBlazor_Server>("blazr-app-mudblazor-server");

builder.AddProject<Projects.Blazr_App_Vanilla_Server>("blazr-app-vanilla-server");

builder.Build().Run();
