using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//inspired by https://github.com/alexismorin/Light-Probe-Populator
namespace HappysTools.LightProbePopulator
{
    public class GenerateLightProbesWindow : EditorWindow
    {
        float probeMod = 1.0f;
        int seed = 0;

        void OnGUI()
        {
            GUILayout.Label("Light Probe Populator", EditorStyles.boldLabel);
            GUILayout.Label("Seed: " + seed.ToString());
            if (GUILayout.Button("Randomize Seed"))
            {
                seed = Random.Range(0, 1000000);
            }
            probeMod = EditorGUILayout.Slider("Probe Density", probeMod, 2.0f, 50.0f);
            //round probeMod to int
            probeMod = (int)probeMod;
            if (GUILayout.Button("Generate Light Probes"))
            {
                GenerateLightProbes.generate(probeMod, seed);
            }
        }
    }

    public class GenerateLightProbes : MonoBehaviour
    {
        //create a editor window to generate light probes
        [MenuItem("VRC Packages/Happys Tools/Light Probe Populator")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(GenerateLightProbesWindow));
        }

        public static void generate(float probeMod, int seed)
        {
            Random.InitState(seed);

            GameObject lightProbes;
            List<Vector3> probeLocations = new List<Vector3>();

            if (GameObject.Find("Light Probe Group") != null)
            {
                DestroyImmediate(GameObject.Find("Light Probe Group"));
            }

            lightProbes = new GameObject("Light Probe Group");
            lightProbes.AddComponent<LightProbeGroup>();
            lightProbes.GetComponent<LightProbeGroup>().probePositions = probeLocations.ToArray();

            GameObject[] objectsInScene = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject obj in objectsInScene)
            {
                if (obj.isStatic)
                {
                    if (obj.GetComponent<Renderer>() != null)
                    {
                        //add all 8 corners of the bounding box
                        probeLocations.Add(obj.GetComponent<Renderer>().bounds.max);
                        probeLocations.Add(obj.GetComponent<Renderer>().bounds.min);
                        probeLocations.Add(
                            new Vector3(
                                obj.GetComponent<Renderer>().bounds.max.x,
                                obj.GetComponent<Renderer>().bounds.max.y,
                                obj.GetComponent<Renderer>().bounds.min.z
                            )
                        );
                        probeLocations.Add(
                            new Vector3(
                                obj.GetComponent<Renderer>().bounds.max.x,
                                obj.GetComponent<Renderer>().bounds.min.y,
                                obj.GetComponent<Renderer>().bounds.max.z
                            )
                        );
                        probeLocations.Add(
                            new Vector3(
                                obj.GetComponent<Renderer>().bounds.min.x,
                                obj.GetComponent<Renderer>().bounds.max.y,
                                obj.GetComponent<Renderer>().bounds.max.z
                            )
                        );
                        probeLocations.Add(
                            new Vector3(
                                obj.GetComponent<Renderer>().bounds.min.x,
                                obj.GetComponent<Renderer>().bounds.min.y,
                                obj.GetComponent<Renderer>().bounds.max.z
                            )
                        );
                        probeLocations.Add(
                            new Vector3(
                                obj.GetComponent<Renderer>().bounds.min.x,
                                obj.GetComponent<Renderer>().bounds.max.y,
                                obj.GetComponent<Renderer>().bounds.min.z
                            )
                        );
                        probeLocations.Add(
                            new Vector3(
                                obj.GetComponent<Renderer>().bounds.max.x,
                                obj.GetComponent<Renderer>().bounds.min.y,
                                obj.GetComponent<Renderer>().bounds.min.z
                            )
                        );
                    }

                    //lights
                    if (obj.GetComponent<Light>() != null)
                    {
                        //lights position
                        probeLocations.Add(obj.transform.position);

                        //place probes around the light up to their range
                        //point
                        if (obj.GetComponent<Light>().type == LightType.Point)
                        {
                            //spherically distribute probes around the light
                            for (int i = 0; i < obj.GetComponent<Light>().range * probeMod; i++)
                            {
                                probeLocations.Add(
                                    obj.transform.position
                                        + Random.insideUnitSphere * obj.GetComponent<Light>().range
                                );
                            }
                        }

                        //spot
                        if (obj.GetComponent<Light>().type == LightType.Spot)
                        {
                            //distribute probes in a cone around the light based on its range and spot angle
                            for (int i = 0; i < obj.GetComponent<Light>().range * probeMod; i++)
                            {
                                Vector3 randomPos =
                                    Random.insideUnitSphere * obj.GetComponent<Light>().range;
                                if (
                                    Vector3.Angle(randomPos, obj.transform.forward)
                                    < (obj.GetComponent<Light>().spotAngle / 2)
                                )
                                {
                                    probeLocations.Add(obj.transform.position + randomPos);
                                }
                                else
                                {
                                    i--;
                                }
                            }
                        }

                        //area
                        if (obj.GetComponent<Light>().type == LightType.Area)
                        {
                            //distribute probes in a square away from the light based on its range and area size
                            for (
                                int i = 0;
                                i < obj.GetComponent<Light>().range * (probeMod / 2);
                                i++
                            )
                            {
                                probeLocations.Add(
                                    obj.transform.TransformPoint(
                                        new Vector3(
                                            Random.Range(
                                                -obj.GetComponent<Light>().areaSize.x / 2,
                                                obj.GetComponent<Light>().areaSize.x / 2
                                            ),
                                            Random.Range(
                                                -obj.GetComponent<Light>().areaSize.y / 2,
                                                obj.GetComponent<Light>().areaSize.y / 2
                                            ),
                                            Random.Range(0, obj.GetComponent<Light>().range)
                                        )
                                    )
                                );
                            }
                        }
                    }
                }
            }

            lightProbes.GetComponent<LightProbeGroup>().probePositions = probeLocations.ToArray();

            //select the light probe group
            Selection.activeGameObject = lightProbes;
        }
    }
}
