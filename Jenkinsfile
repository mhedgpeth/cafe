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

stage('publish') {
  parallel windows: {
    node {
        unstash 'everything'
        dir('src/cafe') {
        bat 'dotnet publish -r win10-x64'
      }
    }
  }, centos: {
    node {
        unstash 'everything'
        dir('src/cafe') {
        bat 'dotnet publish -r centos.7-x64'
      }
    }
  }, ubuntu: {
    node {
      unstash 'everything'
      dir('src/cafe') {
        bat 'dotnet publish -r ubuntu.16.04-x64'
      }
    }
  }
}
