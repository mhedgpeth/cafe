node {
  stage('compile') {
    checkout scm
    
    dir('src/cafe') {
      bat 'dotnet build'
    }
  }
}
