include(cuvis_dotnet_wrapper)
if(NOT TARGET cuvis::csharp)
	add_library(cuvis::csharp ALIAS cuvis_dotnet_wrapper)
endif()