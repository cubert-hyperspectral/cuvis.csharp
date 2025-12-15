cmake_minimum_required(VERSION 3.10)


set(target_name cuvis_netstubs)

set(PROJECT_FILES
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_cuvis_data_type_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_cuvis_operation_mode_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_cuvis_hardware_state_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_double.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_f_p_q_const__char_int__void.cs
#${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_f_p_q_const__wchar_t_int__void.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_cuvis_status_t.cs

#${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_f_int_int__void.cs

#${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_f_q_const__cuvis_acquisition_event_t__void.cs
#${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_f_q_const__cuvis_base_event_t__void.cs
#${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_f_q_const__cuvis_component_event_t__void.cs
#${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_f_q_const__cuvis_processing_event_t__void.cs
# ${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_f_q_const__cuvis_quality_event_t__void.cs

${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_int.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_unsigned_char.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_unsigned_int.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_unsigned_long_long.cs
#${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/SWIGTYPE_p_void.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_il.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_ilPINVOKE.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_component_info_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_calibration_info_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_component_type_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_data_type_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_pansharpening_settings_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_export_general_settings_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_export_tiff_settings_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_export_view_settings_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_gps_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_hardware_state_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_imbuffer_format_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_imbuffer_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_loglevel_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_mesu_metadata_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_operation_mode_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_pan_sharpening_algorithm_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_pan_sharpening_interpolation_type_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_proc_args_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_processing_mode_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_reference_type_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_save_args_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_session_info_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_sensor_info_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_status_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_tiff_compression_mode_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_tiff_format_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_view_category_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_view_data_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_viewer_settings_t.cs
#${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/LogHandlerBase.cs
#${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/LocalizedLogHandlerBase.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_event_acquisition_data_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_event_base_data_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_event_component_data_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_event_processing_event_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_event_quality_event_t.cs
#${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/EventCallback.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_worker_settings_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_worker_state_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_session_item_type_t.cs
${CMAKE_BINARY_DIR}/intermediate/cuvis_csil/cuvis_session_merge_mode_t.cs
)


set_source_files_properties(${PROJECT_FILES} PROPERTIES GENERATED TRUE)
add_library(${target_name} SHARED
    	${PROJECT_FILES} 
)


target_compile_definitions(${target_name} PRIVATE -D CUBERT_cbrt_path="${CBRT_PATH}")

set_target_properties(${target_name} PROPERTIES COMMON_LANGUAGE_RUNTIME "")

file(MAKE_DIRECTORY ${CMAKE_BINARY_DIR}/intermediate/cuvis_csharp/lib)
add_custom_command(TARGET ${target_name} POST_BUILD COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:${target_name}> ${CMAKE_BINARY_DIR}/intermediate/cuvis_csharp/lib/ )


target_compile_options(${target_name}
	PRIVATE
		/fp:precise # /fp:strict is incompatible with /clr
		/langversion:latest
		#/Al
)
target_compile_options(${target_name} PRIVATE /fp:precise) # /fp:strict is incompatible with /clr
set_target_properties(${target_name}
	PROPERTIES
	#VS_GLOBAL_ROOTNAMESPACE ${output_name}
	LINKER_LANGUAGE CSharp
	#PROPERTY VS_DOTNET_REFERENCES "System"
)
# Note: Modification of compiler flags is required for CLR compatibility now that we are using .resx files.
string(REPLACE "/EHsc" "" CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS}")
string(REPLACE "/RTC1" "" CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG}")


set_target_properties(${target_name} PROPERTIES FOLDER "${projprefix}/dotnet")


include(cuvis_csil)
add_dependencies(${target_name}  cuvis_csil)

