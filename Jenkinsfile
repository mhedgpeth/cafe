node {
  stage('compile') {
    checkout scm
    stash 'everything'
    dir('src/cafe') {
      bat 'dotnet restore'
      bat 'dotnet build'
    }
  }
}

stage('test') {
    parallel unitTests: {
      node {
        unstash 'everything'
        dir('test/cafe.Test') {
            bat 'dotnet restore'
            bat 'dotnet test'
          }
      }
    }, integrationTests: {
        node {
          unstash 'everything'
          dir('test/cafe.IntegrationTest'){
              bat 'dotnet restore'
              bat 'dotnet test'
          }
        }
    },
    failFast: false
}



          
