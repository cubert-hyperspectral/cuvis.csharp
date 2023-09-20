
set(target_name cuvis_csil)
set(SWIG_LANGUAGE csharp)
set(CMAKE_LANGUAGE CSharp)

file(MAKE_DIRECTORY "${CMAKE_BINARY_DIR}/intermediate/${target_name}")
set(SWIG_OUTPUT_DIR "${CMAKE_BINARY_DIR}/intermediate/${target_name}")

list(APPEND CMAKE_MODULE_PATH
  "${CMAKE_CURRENT_LIST_DIR}/cuvis.swig/")
include(cuvis_swig)

set_target_properties(${target_name} PROPERTIES FOLDER "${projprefix}/dotnet")

