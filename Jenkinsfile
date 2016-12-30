node {
  stage('compile') {
    checkout scm
    
    dir('src/cafe') {
      bat 'dotnet restore'
      bat 'dotnet build'
    }
  }
}
