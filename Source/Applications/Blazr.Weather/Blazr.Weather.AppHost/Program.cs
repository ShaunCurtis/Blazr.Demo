var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Blazr_App_Fluent_Server>("blazr-app-fluent-server");

builder.AddProject<Projects.Blazr_App_Fluent_WASM_Server>("blazr-app-fluent-wasm-server");

builder.Build().Run();
