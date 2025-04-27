resource runHello 'exec' = {
  name: 'RunHelloWorld'
  properties: {
    input: {
      shell: 'bash'
      args: ['-c', 'echo Hello from Orka']
    }
    timeoutSeconds: 60
  }
}