# simple-trace

a simple trace solution for all client language use HTTP protocol.

## system components

### a demo app for how to use it.

- Demo.WinApp : simple demos for invoke test
- Demo.Foo : A demo long running task for demo trace info

### SimpleTrace libs

- Common(SimpleTrace.Common)
- SimpleTrace
- SimpleTrace.Api
- SimpleTrace.Shared

### OpenTrace and Jaeger impl

- SimpleTrace.OpenTrace
- SimpleTrace.OpenTrace.Jaeger (Impl)

### Daemon for SimpleTraceApi and Jaeger running

- SimpleTrace.Server
- SimpleTrace.WindowsService

### share projects for code links

SharedProject.Common(SimpleTrace.Common)
SharedProject.SimpleTrace(SimpleTrace.Shared)

### test project for SimpleTrace

- SimpleTrace.UnitTest

## change list

- 
- 20190927 refact projects structure and namespaces
- 20190827 init projects