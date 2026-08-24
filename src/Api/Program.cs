using Api;

var builder = AppBuilder.Build(args);
var app = await App.Build(builder);

await app.RunAsync();