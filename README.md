## PracticeAssetment
To provide information to CQC UI

### How to build and run locally
Build and run *PracticeAssetment* from Visual Studio

## PracticeAssessment.Migration 
```
PracticeAssessment.Migration
Set PracticeAssessment.Migration as startup project, then run this Project,
This project will generate the Sql server database

Note: Please update the DATABASE_CONNECTION in the launchSettings.json file
```
## PracticeAssessment 
```
Set PracticeAssessment as startup project. Then run it
Note: please update the ConnectionStrings form appconfig.json file

### How to build and run in docker

Build with
```
>docker build ./ -t PracticeAssetment
```

Run docker container locally:
```
docker run --rm -it -p 8080:8080 PracticeAssetment
```