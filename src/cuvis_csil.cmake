
set(target_name cuvis_csil)
set(SWIG_LANGUAGE csharp)
set(CMAKE_LANGUAGE CSharp)

set(SWIG_OUTPUT_DIR "${EXECUTABLE_OUTPUT_PATH}/intermediate/${target_name}")

list(APPEND CMAKE_MODULE_PATH
  "${CMAKE_CURRENT_LIST_DIR}/../../../swig/cmake/")
include(cuvis_swig)

