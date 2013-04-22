####################################################################
#
# CMake Build Script for libAuto, which basically is auto 2000 for 
# use with in the Auto2000 AUTO module
#
# Author: Frank T. Bergmann
#

cmake_minimum_required(VERSION 2.8)
project(Auto)

include (CMakeTestCCompiler)
include (CheckCSourceCompiles)
include (CheckCXXSourceCompiles)
include (CheckStructHasMember)
include (CheckLibraryExists)
include (CheckFunctionExists)
include (CheckCCompilerFlag)
include (CheckCSourceRuns)
include (CheckSymbolExists)
include (CheckTypeSize)


####################################################################
#
# Set up version information, it might be better to just write it here
# rather than parsing VERSION.txt
# 
SET(AUTO_VERSION_MAJOR 2)
SET(AUTO_VERSION_MINOR 0)
SET(AUTO_VERSION_PATCH 0)
SET(AUTO_VERSION_RELEASE "")

SET(AUTO_VERSION_NUMERIC 20100)
SET(PACKAGE_VERSION "${AUTO_VERSION_MAJOR}.${AUTO_VERSION_MINOR}.${AUTO_VERSION_PATCH}${AUTO_VERSION_RELEASE}")


####################################################################
#
# the next lines configure the parameters for packaging the binaries
# they can be invoked with: make package / nmake package or by using
# cpack -G zip|deb|rpm|dmg|nsis
#

INCLUDE(InstallRequiredSystemLibraries)

SET(CPACK_PACKAGE_DESCRIPTION_SUMMARY "The AUTO library")
SET(CPACK_PACKAGE_NAME "AUTO")
SET(CPACK_PACKAGE_VENDOR "Frank T. Bergmann")
SET(CPACK_PACKAGE_CONTACT "Frank T. Bergmann <sbwteam@gmail.com>")
SET(CPACK_PACKAGE_VERSION_MAJOR "${AUTO_VERSION_MAJOR}")
SET(CPACK_PACKAGE_VERSION_MINOR "${AUTO_VERSION_MINOR}")
SET(CPACK_PACKAGE_VERSION_PATCH "${AUTO_VERSION_PATCH}")
SET(CPACK_SOURCE_PACKAGE_FILE_NAME  "libauto-${AUTO_VERSION_MAJOR}.${AUTO_VERSION_MINOR}.${AUTO_VERSION_PATCH}")
set(CPACK_SOURCE_IGNORE_FILES ".svn/;/build/;.*~")
include(CPack)

# set build type default
set(CMAKE_BUILD_TYPE "Release" CACHE STRING "Choose the type of build, options are: None (CMAKE_CXX_FLAGS or CMAKE_C_FLAGS are used), Debug, Release, RelWithDebInfo, MinSizeRel")


####################################################################
#
# Need some variables set up, such as the name for the libSBML
# library and the Path and file separator characters
#

if(UNIX)
	set(PATH_SEP "/")
	set(FILE_SEP ":")
	set(AUTO_LIBRARY Auto)
else()
	set(PATH_SEP "\\")
	set(FILE_SEP ";")
	if (MINGW)
		set(AUTO_LIBRARY Auto)
	else()
		set(AUTO_LIBRARY libAuto)
	endif()
endif()

####################################################################
#
# Set up remaining variables, add option for universal binaries
#  
set(BUILD_DEFINITIONS)  
if(UNIX)
    add_definitions(-DLINUX)
    set(BUILD_DEFINITIONS "${BUILD_DEFINITIONS} -DLINUX")
    if(APPLE)
      add_definitions(-DDARWIN)
      set(BUILD_DEFINITIONS "${BUILD_DEFINITIONS} -DDARWIN")
      # On OSX it is common to build universal binaries to support multiple
        # processor architectures. The default behavior is not to build
        # multiple architectures, as most users might not need that.
        option(ENABLE_UNIVERSAL "Create universal binaries on Mac OS X." OFF)

        set(CMAKE_OSX_ARCHITECTURES "${CMAKE_OSX_ARCHITECTURES}" CACHE STRING
           "A semicolon-separated list of build architectures to be used.")
        if(ENABLE_UNIVERSAL)
            # if universal binaries are requested and none defined so far
            # overwrite them with all three common architectures. If the user
            # specified their own list of architectures do not touch!
            if(CMAKE_OSX_ARCHITECTURES STREQUAL "")
                STRING(REGEX REPLACE "^.*MacOSX([0-9]*\\.[0-9]*)\\.sdk$" "\\1"
                       OSX_SDK_VERSION "${CMAKE_OSX_SYSROOT}")
                if(OSX_SDK_VERSION  VERSION_EQUAL "10.7" OR OSX_SDK_VERSION  VERSION_EQUAL "10.8")                   # OSX Lion no longer supports ppc architecture
                   set(CMAKE_OSX_ARCHITECTURES "i386;x86_64" CACHE STRING
                      "A semicolon-separated list of build architectures to be used." FORCE)
                else()
                   set(CMAKE_OSX_ARCHITECTURES "i386;ppc;x86_64" CACHE STRING
                       "A semicolon-separated list of build architectures to be used." FORCE)
                endif()
            endif()
        endif(ENABLE_UNIVERSAL)
    else()
      set(AUTO_LIBS ${AUTO_LIBS} 'pthread')
    endif(APPLE)
    add_definitions( -DPACKAGE_VERSION=\"${PACKAGE_VERSION}\"  -DPACKAGE_NAME=\"${PROJECT_NAME}\")
    set(BUILD_DEFINITIONS "${BUILD_DEFINITIONS} -DPACKAGE_VERSION=\"${PACKAGE_VERSION}\"  -DPACKAGE_NAME=\"${PROJECT_NAME}\"")

	if(NOT CYGWIN)
            # on cygwin all code is position independent so -fPIC is not needed
            set (CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -fno-strict-aliasing -fPIC")
            set (CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -fno-strict-aliasing -fPIC")
    endif()
	
else(UNIX)

    set(USING_INTEL)
    if (WIN32 AND CMAKE_C_COMPILER AND ${CMAKE_C_COMPILER} MATCHES ".*icl.*$")
      message(STATUS "Detected Intel Compiler")
      set(USING_INTEL TRUE)
    endif ()
	
	add_definitions(-DWIN32)
	set(BUILD_DEFINITIONS "${BUILD_DEFINITIONS} -DWIN32 ")
	if(MSVC OR USING_INTEL)
		add_definitions(-D_CRT_SECURE_NO_WARNINGS -D_CRT_NONSTDC_NO_DEPRECATE)
		set(BUILD_DEFINITIONS "${BUILD_DEFINITIONS} -D_CRT_SECURE_NO_WARNINGS")
		option(WITH_STATIC_RUNTIME "Compile using the static MSVC Runtime" OFF)
		if (WITH_STATIC_RUNTIME)
			foreach(flag_var
				CMAKE_CXX_FLAGS CMAKE_CXX_FLAGS_DEBUG CMAKE_CXX_FLAGS_RELEASE
				CMAKE_CXX_FLAGS_MINSIZEREL CMAKE_CXX_FLAGS_RELWITHDEBINFO)

				if(${flag_var} MATCHES "/MD")
					string(REGEX REPLACE "/MD" "/MT" ${flag_var} "${${flag_var}}")
				endif(${flag_var} MATCHES "/MD")
			endforeach(flag_var)
		add_definitions( -D_MT)
		endif(WITH_STATIC_RUNTIME)

		file(GLOB WIN32_BINARIES ${CMAKE_SOURCE_DIR}/dependencies/bin/*.dll)
		INSTALL(FILES ${WIN32_BINARIES} DESTINATION bin)

	elseif(CYGWIN)
		add_definitions(-DCYGWIN)
		set(BUILD_DEFINITIONS "${BUILD_DEFINITIONS} -DCYGWIN")
	endif(MSVC OR USING_INTEL)

endif(UNIX)




####################################################################
#
# Find all sources 
#
	
file(GLOB AUTO_SOURCES RELATIVE ${CMAKE_CURRENT_SOURCE_DIR} 
	./*.c* ./libf2c/*.c* *.h
	)

	
# remove stdafx file to remove warning
if (NOT WIN32)
	list(REMOVE_ITEM AUTO_SOURCES "stdafx.cpp" )
endif()
	
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/)	
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/libf2c)	
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../include)	
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../include/AUTO)	

####################################################################
#
# Build shared library
# 
option(WITH_BUILD_SHARED "Build the shared AUTO library" ON)

if (WITH_BUILD_SHARED)

	add_library (AUTO SHARED ${AUTO_SOURCES})
	target_link_libraries(AUTO ${AUTO_LIBS})
	set_target_properties (AUTO PROPERTIES OUTPUT_NAME "${AUTO_LIBRARY}")
	set_target_properties(AUTO PROPERTIES VERSION "${AUTO_VERSION_MAJOR}.${AUTO_VERSION_MINOR}.${AUTO_VERSION_PATCH}")
	set_target_properties(AUTO PROPERTIES SOVERSION "${AUTO_VERSION_MAJOR}.${AUTO_VERSION_MINOR}")
	
	INSTALL(TARGETS AUTO
		RUNTIME DESTINATION bin
		LIBRARY DESTINATION lib
		ARCHIVE DESTINATION lib
	)

endif(WITH_BUILD_SHARED)

####################################################################
#
# Build static library
# 
option(WITH_BUILD_STATIC "Build the static AUTO library" OFF)

if (WITH_BUILD_STATIC)

	add_library (AUTO-static STATIC ${AUTO_SOURCES} )
	if (WIN32 AND NOT CYGWIN)
	
		# don't decorate static library 
		SET_TARGET_PROPERTIES(AUTO-static PROPERTIES COMPILE_DEFINITIONS "AUTO_STATIC=1")
		
	endif(WIN32 AND NOT CYGWIN)
	set_target_properties(AUTO-static PROPERTIES OUTPUT_NAME "${AUTO_LIBRARY}-static")
	target_link_libraries(AUTO-static ${AUTO_LIBS})
	
	INSTALL(TARGETS AUTO-static 
		RUNTIME DESTINATION bin
		LIBRARY DESTINATION lib
		ARCHIVE DESTINATION lib
	)

endif (WITH_BUILD_STATIC)


file(GLOB HEADER_FILES ${CMAKE_CURRENT_SOURCE_DIR}/include/auto/*.h )
install(FILES ${HEADER_FILES} DESTINATION include/auto)
