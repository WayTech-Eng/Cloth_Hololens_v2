using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Obi
{

    public class ObiParticlePicker : MonoBehaviour
    {

        public class ParticlePickEventArgs : EventArgs
        {

            public int particleIndex;
            public Vector3 worldPosition;

            public ParticlePickEventArgs(int particleIndex, Vector3 worldPosition)
            {
                this.particleIndex = particleIndex;
                this.worldPosition = worldPosition;
            }
        }

        [Serializable]
        public class ParticlePickUnityEvent : UnityEvent<ParticlePickEventArgs> { }

        public ObiSolver solver;
        public float radiusScale = 1;

        public ParticlePickUnityEvent OnParticlePicked;
        public ParticlePickUnityEvent OnParticleHeld;
        public ParticlePickUnityEvent OnParticleDragged;
        public ParticlePickUnityEvent OnParticleReleased;

        private Vector3 lastMousePos = Vector3.zero;
        private int pickedParticleIndex = -1;
        private float pickedParticleDepth = 0;

        private Vector3 pickLocation;  // Mouse pick location
        private Vector3 trailingLocation;
        private Vector3 endLocation;

        private int counter = 0;
        private DateTime startTime, endTime;
        private int steps = 0;
        private float movement = 0;
        private int caseswitch = 0;

        private float[] arrx = new float[50];
        private float[] arry = new float[50];
        private float[] arrz = new float[50];

        void Awake()
        {
            lastMousePos = Input.mousePosition;
        }

        void LateUpdate()
        {
            //Theirs();
            Mine();
            lastMousePos = Input.mousePosition;
        }

        void Theirs()
        {
            if (solver != null)
            {

                // Click:
                if (Input.GetMouseButtonDown(0))
                {

                    pickedParticleIndex = -1;

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    float closestMu = float.MaxValue;
                    float closestDistance = float.MaxValue;

                    Matrix4x4 solver2World = solver.transform.localToWorldMatrix;

                    // Find the closest particle hit by the ray:
                    for (int i = 0; i < solver.renderablePositions.count; ++i)
                    {

                        Vector3 worldPos = solver2World.MultiplyPoint3x4(solver.renderablePositions[i]);

                        float mu;
                        Vector3 projected = ObiUtils.ProjectPointLine(worldPos, ray.origin, ray.origin + ray.direction, out mu, false);
                        float distanceToRay = Vector3.SqrMagnitude(worldPos - projected);

                        // Disregard particles behind the camera:
                        mu = Mathf.Max(0, mu);

                        float radius = solver.principalRadii[i][0] * radiusScale;

                        if (distanceToRay <= radius * radius && distanceToRay < closestDistance && mu < closestMu)
                        {
                            closestMu = mu;
                            closestDistance = distanceToRay;
                            pickedParticleIndex = i;
                        }
                    }
                    print("---------------");
                    print(solver.renderablePositions[pickedParticleIndex]);
                    print(pickedParticleIndex);

                    if (pickedParticleIndex >= 0)
                    {
                        pickedParticleDepth = Camera.main.transform.InverseTransformVector(solver2World.MultiplyPoint3x4(solver.renderablePositions[pickedParticleIndex]) - Camera.main.transform.position).z;

                        if (OnParticlePicked != null)
                        {
                            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepth));
                            OnParticlePicked.Invoke(new ParticlePickEventArgs(pickedParticleIndex, worldPosition));
                            print(worldPosition);
                        }
                    }

                }
                else if (pickedParticleIndex >= 0)
                {

                    // Drag:
                    Vector3 mouseDelta = Input.mousePosition - lastMousePos;
                    if (mouseDelta.magnitude > 0.01f && OnParticleDragged != null)
                    {
                        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepth));
                        OnParticleDragged.Invoke(new ParticlePickEventArgs(pickedParticleIndex, worldPosition));
                    }
                    else if (OnParticleHeld != null)
                    {
                        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepth));
                        OnParticleHeld.Invoke(new ParticlePickEventArgs(pickedParticleIndex, worldPosition));
                    }

                    // Release:				
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (OnParticleReleased != null)
                        {
                            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepth));
                            OnParticleReleased.Invoke(new ParticlePickEventArgs(pickedParticleIndex, worldPosition));
                        }

                        pickedParticleIndex = -1;
                    }
                }
            }
        }
        void Mine()
        {
            //print("RUnning...");
            if (solver != null)
            {
                //print("RUnning...");
                var pick = GameObject.Find("Pick").transform.position;
                var end = GameObject.Find("End").transform.position;
                pickLocation = new Vector3(pick.x, pick.y, pick.z);
                endLocation = new Vector3(end.x, end.y + 1, end.z);
                //endLocation = new Vector3(pick.x + 2, pick.y + 2, pick.z);

                // Click:
                if ((Input.GetKey("return")) && (pickedParticleIndex < 0))
                {
                    pickedParticleIndex = -1;

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    float closestMu = float.MaxValue;
                    float closestDistance = float.MaxValue;

                    Matrix4x4 solver2World = solver.transform.localToWorldMatrix;

                    // Find the closest particle hit by the ray:                    
                    print("Pick: ");
                    print(pick);
                    double smallest = float.MaxValue;
                    for (int i = 0; i < solver.renderablePositions.count; ++i)
                    {
                        Vector3 worldPos = solver2World.MultiplyPoint3x4(solver.renderablePositions[i]);
                        double dx = pick.x - worldPos.x;
                        double dz = pick.z - worldPos.z;
                        double dist = Math.Sqrt(dx * dx + dz * dz);

                        if (dist < smallest)
                        {
                            smallest = dist;
                            pickedParticleIndex = i;
                        }
                    }

                    if (pickedParticleIndex >= 0)
                    {
                        if (OnParticlePicked != null)
                        {
                            trailingLocation = new Vector3(pick.x, pick.y, pick.z);

                            OnParticlePicked.Invoke(new ParticlePickEventArgs(pickedParticleIndex, pickLocation));
                            PathGen();

                            startTime = DateTime.Now;
                        }
                    }
                }
                else if (pickedParticleIndex >= 0)
                {
                    counter++;
                    int len = arrx.Length;
                    // Implement switch case to move in y direction first...
                    // 0 .... move up
                    // 1 .... move across
                    // 2 .... move down
                    //if (counter >= 10)
                    endTime = DateTime.Now;
                    double elapsed = ((TimeSpan)(endTime - startTime)).TotalMilliseconds;
                    if (elapsed > 25)
                    {
                        switch (caseswitch)
                        {
                            case 0: // Move up
                                trailingLocation = new Vector3(pick.x, arry[steps], pick.z);
                                break;
                            case 1: // Move lateral
                                trailingLocation = new Vector3(arrx[steps], arry[len - 1], arrz[steps]);
                                print("Lateral");
                                break;
                            case 2: // Move down
                                trailingLocation = new Vector3(arrx[len - 1], arry[len - 1 - steps], arrz[len - 1]);
                                print("Down");
                                break;
                        }
                        OnParticleDragged.Invoke(new ParticlePickEventArgs(pickedParticleIndex, trailingLocation));
                        counter = 0;
                        steps++;
                        startTime = DateTime.Now;
                    }
                    else
                    {
                        OnParticleDragged.Invoke(new ParticlePickEventArgs(pickedParticleIndex, trailingLocation));
                    }

                    if (steps >= len - 1)
                    {
                        Vector3 trailingLocation = new Vector3(arrx[steps], arry[steps], arrz[steps]);
                        OnParticleReleased.Invoke(new ParticlePickEventArgs(pickedParticleIndex, trailingLocation));
                        //print("Moving");
                        steps = 0;
                        movement = 0;
                        caseswitch++;
                        if (caseswitch > 2)
                        {
                            caseswitch = 0;
                            pickedParticleIndex = -1;
                        }
                    }

                }
            }
        }

        // Path generation function
        void PathGen()
        {
            // sizes
            float len = arrx.Length;
            float x = endLocation.x - pickLocation.x;
            float y = endLocation.y - pickLocation.y;
            float z = endLocation.z - pickLocation.z;
            // Steps
            float xStep = x / len;
            float yStep = y / len;
            float zStep = z / len;

            for (int i = 0; i < len; i++)
            {
                arrx[i] = pickLocation.x + xStep * (i + 1);
                arry[i] = pickLocation.y + yStep * (i + 1);
                arrz[i] = pickLocation.z + zStep * (i + 1);
                //print(arry[i]);
            }
            print("Path Generated!");
        }
    }
}
