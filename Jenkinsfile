node {
  stage('compile') {
    checkout scm
    
    dir('src/cafe') {
      bat 'dotnet restore'
      bat 'dotnet build'
    }
  }
}

stage('test') {
    node {
        dir('test/cafe.Test') {
            bat 'dotnet restore'
            bat 'dotnet test'
        }
    }
}