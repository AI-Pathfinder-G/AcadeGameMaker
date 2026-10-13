# Q0 자식 채널 설치 API 근거

상태: **읽기 전용 설계 근거**. 기존 [신뢰 자식 실행 초안](2026-10-02-q0-trusted-child-execution-contract-draft.md) SHA-256 `E15733998623D0261D3104773D23321049B94CF3C569856305EA4E0BF7B6B1E4`의 미확인 API 관문에 한정한다. 새 의미 승인, 채널 구현, 키 발급, 실제 아홉 선택 실행은 아니다. 현재 승인된 `ExecutionAuthorized=false` 발급은 이 후속 API 확인과 독립적이다.

## 설치 참조와 공식 호출 형태

| 대상 | 설치 참조 확인 | 호출 근거와 한계 |
|---|---|---|
| PowerShell 7.6.5 / .NET 10.0.11 | `pwsh` 실제 위치 `C:\Users\me\.cache\codex-runtimes\codex-primary-runtime\dependencies\native\powershell\pwsh.exe`; 설치 `Microsoft.NETCore.App.Ref/10.0.12/ref/net10.0/System.IO.Pipes.xml` SHA-256 `758A3F29D0199E0F02361ACB33B37C149378E8989F57067B7A9714FA5A119985`, `System.IO.Pipes.dll` SHA-256 `0B7AAA8FEA2A9483857B2AC918814AD1D8FF3303C98D6EF71D4593B16ADEE9BA` | XML 165, 386, 651, 1156행에 `NamedPipeClientStream`, `NamedPipeServerStream`, `WaitForConnection`, `PipeStream.SafePipeHandle`; 326~339행에 유한 시간 `Connect` 오버로드. 공식 [클라이언트](https://learn.microsoft.com/en-us/dotnet/api/system.io.pipes.namedpipeclientstream?view=net-10.0), [서버](https://learn.microsoft.com/en-us/dotnet/api/system.io.pipes.namedpipeserverstream?view=net-10.0), [안전 핸들](https://learn.microsoft.com/en-us/dotnet/api/system.io.pipes.pipestream.safepipehandle?view=net-10.0) 문서와 부합한다. |
| Windows 클라이언트 PID | [공식 Win32 명세](https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-getnamedpipeclientprocessid) 32~69행 | `BOOL GetNamedPipeClientProcessId(HANDLE Pipe, PULONG ClientProcessId)`는 **서버 측** `CreateNamedPipe` 인스턴스 핸들을 받으며 `Kernel32.dll`에 있다. 부모가 연결 중인 `NamedPipeServerStream.SafePipeHandle`로 조회하는 방식은 구현 시 실제 Windows 핸들 연결과 오류 반환을 검증해야 한다. Unity 측 P/Invoke를 전제하지 않는다. |
| PID 재사용 방지 | 설치 `System.Diagnostics.Process.xml` SHA-256 `425D5AE9719ED3C87745DF50AD5AF5D7CE9AEBCF453CB72D284DC5FFEA311338`의 `GetProcessById` 126행, `StartTime` 704행; 공식 [시작 시각](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process.starttime?view=net-10.0), [Win32_Process](https://learn.microsoft.com/en-us/windows/win32/cimwin32prov/win32-process) | `Process.StartTime`은 로컬 살아 있는 프로세스에서 읽고, `Win32_Process`는 `ProcessId`, `ParentProcessId`, `CreationDate`를 제공한다. PID 숫자 단독은 신원 증명이 아니다. 조회 실패·종료·시각 불일치는 거절한다. |
| Unity 6000.6.0f1 | `Editor/Data/UnityReferenceAssemblies/unity-4.8-api/Facades/System.IO.Pipes.dll` SHA-256 `313DB96A0282DE9376C471F5AF64075F22B101544AFFEE0BB781EEE6DFE9B897`; `Editor/Data/MonoBleedingEdge/lib/mono/4.8-api/System.Core.dll` SHA-256 `B06C0C07FFD912013CC35987D05CE9E6415CC04882B4B9B5C8EBF8A65FBEA556`; 같은 위치의 파이프 파사드 SHA-256 `9990A778D9C84DA552114B2922789767334E8EA24BB9370E62CFC50618029549` | 참조 파일의 존재는 확인했다. 해당 프로젝트 시험 asmdef의 `NamedPipeClientStream` 실제 컴파일·Unity 편집기 실행 가능성은 **아직 검증하지 않았다**. 후속 계약에서 격리 복제의 한정 컴파일과 실패 종료를 필수 관문으로 둔다. 파사드 존재만으로 런타임 지원 완료를 주장하지 않는다. |

## 후속 구현 계약에 필요한 최소 경계

1. 신뢰 부모가 로컬 단일 서버 인스턴스를 열고 자신이 시작한 큐·실행자·Unity 자식의 `(PID, UTC 생성 시각, 부모 PID·생성 시각 계보)`를 프로세스 핸들이 유효한 동안 보유한다. 자식은 유한 대기로 연결한다. 부모는 **실제 연결 서버 핸들**에서 얻은 클라이언트 PID와 OS 생성 시각·계보를 자체 시작 원장과 대조한다. 클라이언트가 보낸 PID, 경로, 환경값은 신원 근거가 아니다. 서버 핸들 조회 실패 또는 프로세스 수명 경합은 실패 종료한다.
2. 신원 확인 뒤에도 같은 `InvocationId`, 원시 `PlanSHA`, 정확 `Role`·`Stem`, 현재 권위 세대 및 일회 도전값을 결속한다. 부모는 도전값을 `(InvocationId, Role, Stem, 연결 프로세스 신원)`에 대해 한 번만 발급·소비하고, 중복 연결·응답·역할 교차·부모 종료를 거절한다. PID+생성 시각은 재사용 위험을 줄이는 OS 신원 자료이고 서명 계획이나 일회 소비를 대체하지 않는다.
3. 자식 C# 네 경로와 PowerShell 다섯 경로는 계획·서명·Git 청결·정확 입력을 **각자 재검사**한다. 파이프는 부모 수동 판정의 제한 전달 수단이며, 자식의 임의 `Approved=true`나 일치 SHA만으로 권위가 생기지 않는다. 두 인자 C# 단독 호출은 기존 종료 2를 유지한다. 같은 계정 악성 프로세스에 대한 강한 격리는 범위 밖이다.
4. 후속 승인 전에 `NamedPipeServerStream.SafePipeHandle`의 Windows 실제 연결 PID 조회, Unity 시험 조립 컴파일, 생성 시각·부모 계보 조회를 격리 합성 양성/음성으로 확인해야 한다. 중단, PID 재사용 또는 불명확한 계보에서는 실행 승인과 nonce 재사용을 허용하지 않는다. 이 문서는 그 시험을 수행하지 않았다.

조회는 설치 파일·공식 문서의 정적 확인으로 제한했다. 파이프 생성, Unity 실행, 코드·기존 문서 수정은 하지 않았다.
