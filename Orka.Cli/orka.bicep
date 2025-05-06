resource runHello 'exec' = {
  name: 'RunHelloWorld'
  properties: {
    input: {
      shell: 'powershell'
      args: [
          "Write-Output 'Hello from Orka'"
      ]
      timeoutSeconds: 60
    }
  }
}