
if(NOT TARGET cuvis::csharp)
	include(cuvis_dotnet_wrapper)
	add_library(cuvis::csharp ALIAS cuvis_dotnet_wrapper)
endif()