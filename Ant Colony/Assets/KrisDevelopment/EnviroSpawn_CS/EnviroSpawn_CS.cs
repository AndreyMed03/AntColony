using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace KrisDevelopment.EnvSpawn
{

	[AddComponentMenu("Enviro Spawn/ Enviro Spawn C#")]
	public class EnviroSpawn_CS : MonoBehaviour
	{
		public Color gizmoColor = new Color(0, 0, 1);
		public GameObject[] prefabs;
		public int population = 1;
		public Vector2 dimensions = new Vector2(2, 2);
		public Vector2 scaleVariation = new Vector2(0.5f, 1.5f);
		public Vector2 rotationVariation = new Vector2(0, 360);
		public LayerMask ignoreMask = 0;
		public LayerMask avoidMask = 0;
		public float offset = 0;
		[HideInInspector]
		public GameObject[] instanceObjects;
		[HideInInspector]
		public Vector3[]
			raycastPositions,
			raycastPositionsBeta;
		//[HideInInspector]
		public Mask bitmask;
		public bool followNormalsOrientation = true;

		[HideInInspector]
		public bool fixedPositioning;
		[HideInInspector]
		public bool offsetInEachCell = true;
		[HideInInspector]
		public float fixedGridScale = 1;
		[HideInInspector]
		public int scatterMode = 0; //random, fixed, equal

		[HideInInspector]
		public float[] yRotations;

		[HideInInspector]
		public bool cCheck = false;

		void OnDrawGizmos()
		{
			Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
			Gizmos.matrix = rotationMatrix;

			Gizmos.color = gizmoColor;
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(dimensions.x * transform.localScale.x, 0.1f, dimensions.y * transform.localScale.z));
			Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, gizmoColor.a * 0.1f);
			Gizmos.DrawCube(Vector3.zero, new Vector3(dimensions.x * transform.localScale.x, 0.1f, dimensions.y * transform.localScale.z));

#if UNITY_EDITOR
			if (Selection.Contains(gameObject))
				if (Application.isEditor)
					UpdateData();
#endif
		}

		/// <summary>
		/// Note: Changed to static 
		/// </summary>
#if UNITY_EDITOR
		[MenuItem("Tools/Kris Development/Enviro Spawn Re-Generate All")]
#endif
		public static void MassInstantiateNew()
		{
			EnviroSpawn_CS[] objs = GameObject.FindObjectsOfType<EnviroSpawn_CS>();
			for (uint i = 0; i < objs.Length; i++)
			{
				objs[i].InstantiateNew();
				objs[i].UpdateData();
				objs[i].UpdateData();
			}
		}

        public void InstantiateNew()
        {
            cCheck = false;
            Reset();

            raycastPositions = new Vector3[population];
            raycastPositionsBeta = new Vector3[population];
            instanceObjects = new GameObject[population];
            yRotations = new float[population];
            bitmask = new Mask(population);


            GenerateRaycastPoints();

            for (uint p = 0; p < population; p++)
            {
                if (!instanceObjects[p])
                {
                    if (prefabs.Length > 0)
                    {
                        int randomInstId = Random.Range(0, prefabs.Length);
                        GameObject prop = null;
                        if (prefabs[randomInstId])
                        {
                            bool editor = false;
                            editor = true;


                            prop = (GameObject)PrefabUtility.InstantiatePrefab(prefabs[randomInstId]);
                            prop.transform.position = raycastPositionsBeta[p];
                            prop.transform.rotation = Quaternion.identity;
                            if (!editor)
                            {
                                prop = (GameObject)Instantiate(prefabs[randomInstId], raycastPositionsBeta[p], Quaternion.identity);
                            }
                        }
                        if (prop)
                        {
                            yRotations[p] = Random.Range(rotationVariation.x, rotationVariation.y);

                            float _scale = Random.Range(scaleVariation.x, scaleVariation.y);
                            prop.transform.localScale = new Vector3(_scale, _scale, _scale);
                            prop.transform.parent = this.transform;
                            instanceObjects[p] = prop;
                        }
                    }
                }
            }
            UpdateData();
        }
        private void UpdateData()
		{

			/* NEW VERSION VARIABLE UPDATE PROCEDURE */
			//if the old 'fixed grid variable(bool) ' is used, transfer the information to the new one without damage done to the user.
			if (fixedPositioning)
			{
				fixedPositioning = false;
				scatterMode = 1;
			}
			/*---------------------------------------*/


			//Debug.Log("update data called");
			if (population < 0)
			{
				population = 0;
				return;
			}

			if (raycastPositions != null)
				if (raycastPositions.Length != population)
				{ //Instantiate
					InstantiateNew();
				}
				else
				{
					bitmask = new Mask(population);
					for (int r = 0; r < population; r++)
					{
						RaycastHit hit;
                        if (!Raycast(out hit, r))
                        {
                            // ���� ��� �� ������� ������ ��� �������� Avoid Mask, ���������� ����������
                            raycastPositionsBeta[r] = transform.position + raycastPositions[r];
                        }
                        else if (bitmask != null)
                        {
                            if (bitmask.EvaluateAll())
                            {
                                // ���������� ����� ��������� �������, ���� Avoid Mask ������������ ����������
                                while (!Raycast(out hit, r))
                                {
                                    raycastPositions[r] = GenerateRandomRaycast();
                                }
                            }
                        }

                        Quaternion normalQuaternion = Quaternion.FromToRotation(Vector3.up, hit.normal);
						if (!followNormalsOrientation) //if follow normals is :off: reset the rotation
							normalQuaternion = Quaternion.identity;

						if (instanceObjects.Length > 0)
							if (instanceObjects[r])
							{
								if (bitmask != null)
									instanceObjects[r].SetActive(bitmask.Evaluate(r));
								instanceObjects[r].transform.position = raycastPositionsBeta[r] + hit.normal * offset;
								instanceObjects[r].transform.eulerAngles = new Vector3(normalQuaternion.eulerAngles.x, normalQuaternion.eulerAngles.y, normalQuaternion.eulerAngles.z);
								instanceObjects[r].transform.Rotate(0, yRotations[r], 0, Space.Self);
							}
					}
				}
		}

        private bool Raycast(out RaycastHit hit, int r)
        {
            // ��������� ��������� ����� ����
            Vector3 rayOrigin = transform.position + transform.right * raycastPositions[r].x
                                + transform.up * raycastPositions[r].y
                                + transform.forward * raycastPositions[r].z;

            // ��������� ��� ����
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, ~(ignoreMask)))
            {
                raycastPositionsBeta[r] = hit.point;

                // ��������� Avoid Mask � ���� ���� ������� ���������, ���������� ����������
                if ((avoidMask.value & (1 << hit.transform.gameObject.layer)) != 0)
                {
                    Debug.Log($"Avoid Mask hit: {hit.transform.name}, layer: {hit.transform.gameObject.layer}");
                    bitmask?.Set(r, 0); // ������������ ������� �������
                    return false;
                }

                // ���� Avoid Mask �� ��������, ���������� ������
                bitmask?.Set(r, 1);
                return true;
            }

            // ���� ��� ������ �� �������, ������������ �������
            bitmask?.Set(r, 0);
            return false;
        }


        private Vector3 GenerateRandomRaycast()
        {

            // ��������� ��������� ������� � �������� ��������
            float scaledX = dimensions.x * transform.localScale.x;
            float scaledZ = dimensions.y * transform.localScale.z;



            // ��������� ������� �� ���� X � Z � �������� �������� ��������
            Vector3 rayPos = new Vector3(
                Random.Range(-scaledX / 2, scaledX / 2),  // ��������� ��������� ���������� �� X
                0,  // ������� �� Y ������� �������, ��� ��� ������� ������������ �� ���������
                Random.Range(-scaledZ / 2, scaledZ / 2)   // ��������� ��������� ���������� �� Z
            );

            return rayPos;
        }


        private void GenerateRaycastPoints()
		{
			float x = dimensions.x;
			float y = dimensions.y;
			int lc = 0; //loop count

			if (scatterMode == 0)
				for (uint r = 0; r < population; r++)
				{
					raycastPositions[r] = GenerateRandomRaycast();
				}

			if (scatterMode == 1)
			{
				float tp = float.Parse("" + population); //r
				float c = tp / float.Parse("" + x * y); //expected cycles
				cCheck = false; if (Mathf.Floor(c) > 0) cCheck = true;
				lc = 0;
				for (uint cn = 0; cn < c; cn++) //na - cycle number
				{
					float localCellOffset = fixedGridScale / c * cn; //p
					for (uint ay = 0; ay < y; ay++)
						for (uint ax = 0; ax < x; ax++)
						{
							if (lc < raycastPositions.Length) raycastPositions[lc] = new Vector3(ax * fixedGridScale - x / 2 + fixedGridScale / 2, 0, ay * fixedGridScale - y / 2 + fixedGridScale / 2);
							lc++;
						}
				}
			}

			if (scatterMode == 2)
			{
				int a = (int)Mathf.Sqrt(float.Parse("" + population) * dimensions.x / dimensions.y); //horizontal cell count
				int b = (int)(float.Parse("" + population) / float.Parse("" + a)); //vertical cell count
				lc = 0;
				for (uint a1 = 0; a1 < a; a1++)
					for (uint b1 = 0; b1 < b; b1++)
					{
						raycastPositions[lc] = new Vector3(dimensions.x / a * a1 - (dimensions.x / 2 - dimensions.x / a / 2), 0, dimensions.y / b * b1 - (dimensions.y / 2 - dimensions.y / b / 2));
						lc++;
					}
			}
		}
		public void Reset()
		{
			if (instanceObjects == null)
				return;

			for (uint n = 0; n < instanceObjects.Length; n++)
			{
				if (instanceObjects[n] != null)
				{
					DestroyImmediate(instanceObjects[n].gameObject);
				}
			}

			raycastPositions = new Vector3[0];
			raycastPositionsBeta = new Vector3[0];
			instanceObjects = new GameObject[0];
		}
	}

	[System.Serializable]
	public class Mask
	{
		public int bitmask = 0;

		public Mask()
		{
			this.bitmask = 0;
		}

		public Mask(int length)
		{
			this.bitmask = 0;
		}


		public bool Evaluate(int i)
		{
			return (bitmask & (1 << i)) != 0;
		}

		/// <summary>
		/// Is any bit up?
		/// </summary>
		public bool EvaluateAll()
		{
			return bitmask > 0;
		}

		/// <summary>
		/// Previously called 'Replace'
		/// </summary>
		public bool Set(int index, int value)
		{
			this.bitmask = bitmask & (~(1 << index)); //clear the bit
			this.bitmask = bitmask | (value << index); //set the bit
			return true;
		}
	}
}
