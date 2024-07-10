var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Blazr_App_Fluent_Server>("FluentUI-Server");

builder.AddProject<Projects.Blazr_App_Fluent_WASM_Server>("FluentUI-WASM-Server");

builder.AddProject<Projects.Blazr_App_MudBlazor_Server>("MudBlazor-Server");

builder.AddProject<Projects.Blazr_App_MudBlazor_WASM_Server>("MudBlazor-WASM-Server");

builder.AddProject<Projects.Blazr_App_Vanilla_Server>("Vanilla-Server");

builder.AddProject<Projects.Blazr_App_Vanilla_WASM_Server>("Vanilla-WASM-Server");

builder.Build().Run();
