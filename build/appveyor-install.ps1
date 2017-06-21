If (-not (Test-Path 'intel_sdk_for_opencl_setup_6.0.0.1049.exe')) {
	Invoke-WebRequest http://registrationcenter-download.intel.com/akdlm/irc_nas/vcp/8539/intel_sdk_for_opencl_setup_6.0.0.1049.exe -OutFile intel_sdk_for_opencl_setup_6.0.0.1049.exe
}

If (-not (Test-Path 'opencl_runtime_16.1.1_x64_setup.msi')) {
	Invoke-WebRequest http://registrationcenter-download.intel.com/akdlm/irc_nas/9022/opencl_runtime_16.1.1_x64_setup.msi -OutFile opencl_runtime_16.1.1_x64_setup.msi
}

.\intel_sdk_for_opencl_setup_6.0.0.1049.exe install --output=output.log --eula=accept | Out-Null
msiexec.exe /i opencl_runtime_16.1.1_x64_setup.msi /qn | Out-Null
