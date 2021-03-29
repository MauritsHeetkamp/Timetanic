using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomPathFinder : MonoBehaviour
{
    [SerializeField] Teleporter[] teleporters;

    [SerializeField] GameObject testPlayer, testTarget;
    [SerializeField] GameObject testIndicator;
    // Start is called before the first frame update
    void Start()
    {
        Path path = GetPath(testPlayer.transform, testTarget.transform.position);


        foreach(Path pathPoint in path.travelledPath)
        {
            Instantiate(testIndicator, pathPoint.location, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public Path GetPath(Transform user, Vector3 targetLocation)
    {
        float defaultDistance = Vector3.Distance(user.position, targetLocation);

        PathRoot root = new PathRoot(user, targetLocation, defaultDistance);

        foreach(Teleporter teleporter in teleporters)
        {
            Path newPath = new Path(new List<Path>(), Vector3.Distance(user.position, teleporter.transform.position), teleporter.transform.position, root);
            root.paths.Add(newPath);
            CalculatePaths(newPath);
        }

        Path selectedPath = null;

        foreach(Path completedPath in root.completedPaths)
        {
            if(completedPath.distanceTravelled + Vector3.Distance(completedPath.teleporterToUse.target.position, targetLocation) < defaultDistance)
            {
                defaultDistance = completedPath.distanceTravelled + Vector3.Distance(completedPath.teleporterToUse.target.position, targetLocation);
                selectedPath = completedPath;
            }
        }

        Path completePath = new Path(selectedPath.travelledPath, 0, targetLocation, null, null);

        return completePath;
    }

    public void CalculatePaths(Path pathToCalculate)
    {
        foreach(Teleporter teleporter in teleporters)
        {
            bool hasBeenAtLocation = false;
            foreach(Path currentPathPart in pathToCalculate.travelledPath)
            {
                if(currentPathPart.location == teleporter.transform.position)
                {
                    hasBeenAtLocation = true;
                    break;
                }
            }

            if (!hasBeenAtLocation)
            {
                Path newPath = new Path(new List<Path>(pathToCalculate.travelledPath), pathToCalculate.distanceTravelled, pathToCalculate.location, pathToCalculate.root, teleporter);
                CalculatePaths(newPath);
            }
            else
            {
                pathToCalculate.root.completedPaths.Add(pathToCalculate);
            }
        }
    }

    public static float GetPathDistance(NavMeshPath[] path)
    {
        float distance = 0;
        for(int i = 0; i < path.Length; i++)
        {
            NavMeshPath thisPath = path[i];

            for(int q = 0; q < thisPath.corners.Length; q++)
            {
                Vector3 cornerLocation = thisPath.corners[q];
                if(q < thisPath.corners.Length - 1)
                {
                    distance += Vector3.Distance(cornerLocation, thisPath.corners[q + 1]);
                }
                else
                {
                    if(i < path.Length - 1 && path[i + 1].corners.Length > 0)
                    {
                        distance += Vector3.Distance(cornerLocation, path[i + 1].corners[0]);
                    }
                }
            }
        }
        return distance;
    }

    public class PathRoot
    {
        Transform user;
        Vector3 targetLocation;
        float defaultDistance;

        int openProcesses;
        public List<Path> paths = new List<Path>();


        public List<Path> completedPaths = new List<Path>();

        public PathRoot(Transform _user, Vector3 _targetLocation, float _defaultDistance)
        {
            user = _user;
            targetLocation = _targetLocation;
            defaultDistance = _defaultDistance;
        }
    }

    public class Path
    {
        public float distanceTravelled;

        public Vector3 location;
        public Teleporter teleporterToUse;
        public PathRoot root;
        public List<Path> travelledPath;

        public Path(List<Path> _travelledPath, float newDistanceTravelled, Vector3 _location, PathRoot _root, Teleporter _teleporterToUse = null)
        {
            location = _location;
            distanceTravelled += newDistanceTravelled;
            travelledPath = _travelledPath;
            teleporterToUse = _teleporterToUse;

            travelledPath.Add(this);

            root = _root;
        }
    }
}
