// This information in this file is from the OpenCL 2.1 documentation.
// Source: all comments from https://www.khronos.org/registry/cl/specs/opencl-2.1.pdf#page=58

namespace NOpenCL
{
    public enum ErrorCode
    {
        /// <summary>
        /// executed successfully
        /// aka CL_SUCCESS 
        /// Raised in: clGetDeviceIDs
        /// </summary>
        Success = 0,
        /// <summary>
        /// ErrorCode if no OpenCL devices that matched device_type were found.
        /// aka CL_OUT_OF_HOST_MEMORY 
        /// Raised in: clGetDeviceIDs
        /// </summary>
        DeviceNotFound = -1,
        /// <summary>
        /// ErrorCode if no devices that match device_type and property values specified in properties are 
        /// currently available.
        /// aka CL_DEVICE_NOT_AVAILABLE
        /// Raised in: clCreateContextFromType
        /// </summary>
        DeviceNotAvailable = -2,
        /// <summary>
        /// ErrorCode if program is created with clCreateProgramWithSource and a compiler is not available 
        /// i.e. CL_DEVICE_COMPILER_AVAILABLE specified in table 4.3 is set to CL_FALSE.
        /// aka CL_COMPILER_NOT_AVAILABLE
        /// Raised in: clBuildProgram clCompileProgram
        /// </summary>
        CompilerNotAvailable = -3,
        /// <summary>
        /// ErrorCode if there is a failure to allocate memory for buffer object.
        /// aka CL_MEM_OBJECT_ALLOCATION_FAILURE
        /// Raised in: clCreateBuffer clCreateSubBuffer clEnqueueReadBuffer clEnqueueWriteBuffer 
        ///   clEnqueueReadBufferRect clEnqueueWriteBufferRect clEnqueueCopyBuffer 
        ///   clEnqueueCopyBufferRect clEnqueueFillBuffer clEnqueueMapBuffer clCreateImage 
        ///   clEnqueueCopyImage clEnqueueFillImage clEnqueueCopyImageToBuffer 
        ///   clEnqueueCopyBufferToImage clEnqueueMapImage clCreatePipe clEnqueueMigrateMemObjects
        ///   clEnqueueNDRangeKernel clEnqueueNativeKernel
        /// </summary>
        MemObjectAllocationFailure = -4,
        /// <summary>
        /// ErrorCode if there is a failure to allocate resources required by the OpenCL implementation on 
        /// the device.
        /// aka CL_OUT_OF_RESOURCES
        /// Raised in: (89 different functions)
        /// </summary>
        OutOfResources = -5,
        /// <summary>
        /// ErrorCode if there is a failure to allocate resources required by the OpenCL implementation on 
        /// the host.
        /// aka CL_OUT_OF_HOST_MEMORY
        /// Raised in: (89 different functions)
        /// </summary>
        OutOfHostMemory = -6,
        /// <summary>
        /// ErrorCode if the CL_QUEUE_PROFILING_ENABLE flag is not set for the command-queue, if the 
        /// execution status of the command identified by event is not CL_COMPLETE or if event refers to 
        /// the clEnqueueSVMFree command or is a user event object
        /// aka CL_PROFILING_INFO_NOT_AVAILABLE
        /// Raised in: clGetEventProfilingInfo
        /// </summary>
        ProfilingInfoNotAvailable = -7,
        /// <summary>
        /// ErrorCode if src_buffer and dst_buffer are the same buffer or subbuffer object and the source 
        /// and destination regions overlap or if src_buffer and dst_buffer are different sub-buffers 
        /// of the same associated buffer object and they overlap. The regions overlap if 
        /// src_offset <= dst_offset <= src_offset + size – 1 or if dst_offset <= src_offset <= 
        /// dst_offset + size – 1.
        /// aka CL_MEM_COPY_OVERLAP
        /// Raised in: clEnqueueCopyBuffer clEnqueueCopyBufferRect clEnqueueCopyImage
        /// </summary>
        MemCopyOverlap = -8,
        /// <summary>
        /// ErrorCode if src_image and dst_image do not use the same image format.
        /// aka CL_IMAGE_FORMAT_MISMATCH
        /// Raised in: clEnqueueCopyImage
        /// </summary>
        ImageFormatMismatch = -9,
        /// <summary>
        /// ErrorCode if the image_format is not supported
        /// aka CL_IMAGE_FORMAT_NOT_SUPPORTED
        /// Raised in: clCreateImage clEnqueueReadImage clEnqueueWriteImage clEnqueueCopyImage 
        /// clEnqueueFillImage clEnqueueCopyImageToBuffer clEnqueueCopyBufferToImage 
        /// clEnqueueMapImage clEnqueueNDRangeKernel
        /// </summary>
        ImageFormatNotSupported = -10,
        /// <summary>
        /// ErrorCode if there is a failure to build the program executable. This error will be returned if 
        /// clBuildProgram does not return until the build has completed.
        /// aka CL_BUILD_PROGRAM_FAILURE
        /// Raised in: clBuildProgram
        /// </summary>
        BuildProgramFailure = -11,
        /// <summary>
        /// ErrorCode if there is a failure to map the requested region into the host address space. This 
        /// error cannot occur for image objects created with CL_MEM_USE_HOST_PTR or CL_MEM_ALLOC_HOST_PTR
        /// aka CL_MAP_FAILURE
        /// Raised in: clEnqueueMapBuffer clEnqueueMapImage
        /// </summary>
        MapFailure = -12,
        /// <summary>
        /// ErrorCode if a sub-buffer object is specified as the value for an argument that is a buffer 
        /// object and the offset specified when the sub-buffer object is created is not aligned 
        /// to CL_DEVICE_MEM_BASE_ADDR_ALIGN value for device associated with queue.
        /// aka CL_MISALIGNED_SUB_BUFFER_OFFSET
        /// Raised in: clCreateSubBuffer clEnqueueReadBuffer clEnqueueWriteBuffer 
        /// clEnqueueReadBufferRect clEnqueueWriteBufferRect clEnqueueCopyBuffer 
        /// clEnqueueCopyBufferRect clEnqueueFillBuffer clEnqueueMapBuffer clEnqueueCopyImageToBuffer 
        /// clEnqueueCopyBufferToImage clEnqueueNDRangeKernel
        /// </summary>
        MisalignedSubBufferOffset = -13,
        /// <summary>
        /// ErrorCode if the read and write operations are blocking and the execution status of any of the 
        /// events in event_wait_list is a negative integer value.
        /// aka CL_EXEC_STATUS_ERROR_FOR_EVENTS_IN_WAIT_LIST
        /// Raised in: clEnqueueReadBuffer clEnqueueWriteBuffer clEnqueueReadBufferRect 
        /// clEnqueueWriteBufferRect clEnqueueMapBuffer clEnqueueReadImage clEnqueueWriteImage 
        /// clEnqueueMapImage clEnqueueSVMMemcpy clEnqueueSVMMap clWaitForEvents
        /// </summary>
        ExecStatusErrorForEventsInWaitList = -14,
        /// <summary>
        /// ErrorCode if there is a failure to compile the program source. This error will be returned if 
        /// clCompileProgram does not return until the compile has completed. 
        /// aka CL_COMPILE_PROGRAM_FAILURE
        /// Raised in: clCompileProgram
        /// </summary>
        CompileProgramFailure = -15,
        /// <summary>
        /// ErrorCode if a linker is not available i.e. CL_DEVICE_LINKER_AVAILABLE specified in table 4.3 
        /// is set to CL_FALSE.
        /// aka CL_LINKER_NOT_AVAILABLE
        /// Raised in: clLinkProgram
        /// </summary>
        LinkerNotAvailable = -16,
        /// <summary>
        /// ErrorCode if there is a failure to link the compiled binaries and/or libraries. 
        /// aka CL_LINK_PROGRAM_FAILURE
        /// Raised in: clLinkProgram
        /// </summary>
        LinkProgramFailure = -17,
        /// <summary>
        /// ErrorCode if the partition name is supported by the implementation but in_device could not be 
        /// further partitioned.
        /// aka CL_DEVICE_PARTITION_FAILED
        /// Raised in:  if the partition name is supported by the implementation but in_device 
        /// could not be further partitioned.
        /// </summary>
        DevicePartitionFailed = -18,
        /// <summary>
        /// ErrorCode if the argument information is not available for kernel.
        /// aka CL_KERNEL_ARG_INFO_NOT_AVAILABLE
        /// Raised in: clGetKernelArgInfo
        /// </summary>
        KernelArgInfoNotAvailable = -19,
        InvalidValue = -30,
        /// <summary>
        /// ErrorCode if device_type is not a valid value.
        /// </summary>
        InvalidDeviceType = -31,
        /// <summary>
        /// ErrorCode if platform is not a valid platform.
        /// </summary>
        InvalidPlatform = -32,
        InvalidDevice = -33,
        /// <summary>
        /// ErrorCode if context is not a valid OpenCL context. 
        /// aka CL_INVALID_CONTEXT
        /// Raised in: (39 different functions)
        /// </summary>
        InvalidContext = -34,
        InvalidQueueProperties = -35,
        InvalidCommandQueue = -36,
        InvalidHostPtr = -37,
        InvalidMemObject = -38,
        InvalidImageFormatDescriptor = -39,
        InvalidImageSize = -40,
        InvalidSampler = -41,
        InvalidBinary = -42,
        /// <summary>
        /// ErrorCode if the build options specified by options are invalid.
        /// aka CL_INVALID_BUILD_OPTIONS
        /// Raised in: clBuildProgram
        /// </summary>
        InvalidBuildOptions = -43,
        /// <summary>
        /// ErrorCode if program is not a valid program object
        /// aka CL_INVALID_PROGRAM
        /// Raised in: clBuildProgram clReleaseProgram clBuildProgram clCompileProgram 
        /// clLinkProgram clGetProgramInfo clGetProgramBuildInfo clCreateKernel 
        /// clCreateKernelsInProgram clEnqueueNDRangeKernel
        /// </summary>
        InvalidProgram = -44,
        /// <summary>
        /// ErrorCode if param_name is CL_PROGRAM_NUM_KERNELS or CL_PROGRAM_KERNEL_NAMES and a successful 
        /// program executable has not been built for at least one device in the list of devices 
        /// associated with program.
        /// aka CL_INVALID_PROGRAM_EXECUTABLE
        /// Raised in: clReleaseProgram clBuildProgram clCompileProgram clLinkProgram 
        /// clGetProgramInfo clGetProgramBuildInfo clCreateKernel clCreateKernelsInProgram
        /// </summary>
        InvalidProgramExecutable = -45,
        /// <summary>
        /// ErrorCode if kernel_name is not found in program.
        /// aka CL_INVALID_KERNEL_NAME
        /// Raised in: clCreateKernel
        /// </summary>
        InvalidKernelName = -46,
        InvalidKernelDefinition = -47,
        InvalidKernel = -48,
        InvalidArgIndex = -49,
        InvalidArgValue = -50,
        /// <summary>
        /// ErrorCode if arg_size does not match the size of the data type for an argument that is not a 
        /// memory object or if the argument is a memory object and arg_size != sizeof(cl_mem) or 
        /// ErrorCode if arg_size is zero and the argument is declared with the local qualifier or if the 
        /// argument is a sampler and arg_size != sizeof(cl_sampler).
        /// aka CL_INVALID_ARG_SIZE
        /// Raised in: clSetKernelArg
        /// </summary>        
        InvalidArgSize = -51,
        InvalidKernelArgs = -52,
        InvalidWorkDimension = -53,
        InvalidWorkGroupSize = -54,
        InvalidWorkItemSize = -55,
        InvalidGlobalOffset = -56,
        InvalidEventWaitList = -57,
        /// <summary>
        /// ErrorCode if event is not a valid user event object. 
        /// aka CL_INVALID_EVENT
        /// Raised in: clSetUserEventStatus clWaitForEvents clGetEventInfo clSetEventCallback 
        /// clRetainEvent clReleaseEvent clGetEventProfilingInfo
        /// </summary>
        InvalidEvent = -58,
        InvalidOperation = -59,
        InvalidGlObject = -60,
        InvalidBufferSize = -61,
        InvalidMipLevel = -62,
        InvalidGlobalWorkSize = -63,
        InvalidProperty = -64,
        InvalidImageDescriptor = -65,
        InvalidCompilerOptions = -66,
        InvalidLinkerOptions = -67,
        /// <summary>
        /// ErrorCode if the partition name specified in properties is CL_DEVICE_PARTITION_BY_COUNTS and the 
        /// number of sub-devices requested exceeds CL_DEVICE_PARTITION_MAX_SUB_DEVICES or the 
        /// total number of compute units requested exceeds CL_DEVICE_PARTITION_MAX_COMPUTE_UNITS 
        /// for in_device, or the number of compute units requested for one or more sub-devices is 
        /// less than zero or the number of sub-devices requested exceeds 
        /// CL_DEVICE_PARTITION_MAX_COMPUTE_UNITS for in_device.
        /// aka CL_INVALID_DEVICE_PARTITION_COUNT
        /// Raised in: clCreateSubDevices
        /// </summary>
        InvalidDevicePartitionCount = -68,
    }
}
