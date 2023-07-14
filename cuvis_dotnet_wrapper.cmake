cmake_minimum_required(VERSION 3.10)

# set the project name

include(cuvis_netstubs)

set(target_name cuvis_dotnet_wrapper)

set(PROJECT_FILES
${CMAKE_CURRENT_LIST_DIR}/src/types.cs
${CMAKE_CURRENT_LIST_DIR}/src/events.cs
${CMAKE_CURRENT_LIST_DIR}/src/Measurement.cs
${CMAKE_CURRENT_LIST_DIR}/src/Calibration.cs
${CMAKE_CURRENT_LIST_DIR}/src/AcquistionContext.cs
${CMAKE_CURRENT_LIST_DIR}/src/SessionFile.cs
${CMAKE_CURRENT_LIST_DIR}/src/ProcessingContext.cs
${CMAKE_CURRENT_LIST_DIR}/src/Viewer.cs
${CMAKE_CURRENT_LIST_DIR}/src/Exporter.cs
${CMAKE_CURRENT_LIST_DIR}/src/Worker.cs
)

add_library(${target_name} SHARED
    	${PROJECT_FILES}
)


target_compile_definitions(${target_name} PRIVATE -D CUBERT_cbrt_path="${CBRT_PATH}")

set_target_properties(${target_name} PROPERTIES COMMON_LANGUAGE_RUNTIME "")


target_compile_options(${target_name}
	PRIVATE
		/fp:precise # /fp:strict is incompatible with /clr
		#/Al
		/langversion:latest
)


#add_custom_command(TARGET ${target_name} POST_BUILD COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:${target_name}> ${EXECUTABLE_OUTPUT_PATH}/sdk/cuvis_csharp/lib/ )


target_compile_options(${target_name} PUBLIC "/unsafe")

set(CMAKE_CSharp_FLAGS "/langversion:latest")

set_property(TARGET ${target_name}
	PROPERTY VS_GLOBAL_ROOTNAMESPACE ${output_name}
	PROPERTY DOTNET_TARGET_FRAMEWORK ${net_framework_version}
	PROPERTY VS_DOTNET_REFERENCES "System;System.Device;System.Drawing"
)

#SET_TARGET_PROPERTIES( ${EXE_NAME} PROPERTIES VS_GLOBAL_TargetFrameworkProfile "Client" )
# Note: Modification of compiler flags is required for CLR compatibility now that we are using .resx files.
string(REPLACE "/EHsc" "" CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS}")
string(REPLACE "/RTC1" "" CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG}")


add_dependencies(${target_name}  cuvis_netstubs)
target_link_libraries(${target_name}  cuvis_netstubs)