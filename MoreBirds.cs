using MelonLoader;
using Placemaker;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreBirds
{
    public class MoreBirdsMain : MelonMod
    {
		//Initial Birds data

		//Stores modified UVs about birds meshes
		private static (Vector2, Mesh[])[] posTextures = new (Vector2, Mesh[])[]
		{
			(new Vector2(-0.15625f, 0f),    new Mesh[7]),
			(new Vector2(-0.25f, 0f),  new Mesh[7]),
			(new Vector2(-0.34375f, 0f),  new Mesh[7]),
			(new Vector2(-0f, 0.203125f),  new Mesh[7])
		};



		//Randomize data for granting birds textures
		public static double[] percentBirds = new double[] { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f };
		private static System.Random rand = new System.Random();

		public static bool NPressed = false;

		private static GameObject birdflock;
		private static Placemaker.Life.BirdFlock birdflockPlace;

		public static Mesh flapMesh;
		public static Mesh flyMesh;
		public static Mesh glideMesh;
		public static Mesh halfSitMesh;
		public static Mesh landMesh;
		public static Mesh sitMesh;
		public static Mesh standMesh;

		public static bool initialized = false;

		//List of birds
		public static List<(MeshFilter, MeshFilter)>[] birds = new List<(MeshFilter, MeshFilter)>[5];
		public static List<MeshFilter> birdFilters = new List<MeshFilter> { };

		//Amount of birds
		public static bool controlAmountBirds = false;
		public static int myAmountBirds=1;

		//Spot one bird
		public static Placemaker.Life.Bird chosenBird;
		public static KeyCode ColorBirdKey;
		public static KeyCode UpRootBirdKey;
		public static KeyCode UpRootAllBirdsKey;

		//MeshCollider check
		public static bool colliderUpToDate = false;


		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			if (sceneName == "Placemaker")
			{
				MelonLogger.Msg("Main scene loaded");

				// Initializing ModUI
				BirdsUI.Initialize(this);
				
			}
		}

		//Birds population
		public static void Populate(int index, float value)
        {
			double thisValue = (double)value;
			double[] copyPercentBirds = new double[5];
			percentBirds.CopyTo(copyPercentBirds, 0);

			//Update of percentBirds
			percentBirds[index] = thisValue;

			//Sum the array
			double sumPercent = 0;
            foreach (double percent in percentBirds)
            {
				sumPercent += percent;
            }

			if (sumPercent > 1f)
            {
				copyPercentBirds.CopyTo(percentBirds, 0);
            }

		}

		//Change the amount of birds on the map
		public static void AddBirds(int nb)
        {
			if (birdflock)
            {
				if (nb > myAmountBirds)
                {
					birdflockPlace.IterateBirdCreation();
					myAmountBirds = birdflockPlace.activeBirdsCount;
				}
				else if (nb < myAmountBirds && birdflockPlace.birds.Count > 0)
                {
                    for (int i = 0; i < myAmountBirds - nb; i++)
                    {
						Placemaker.Life.Bird birdToDelete = birdflockPlace.birds.get_Item(0);
						birdflockPlace.SetLanding(birdToDelete, null);
						birdflockPlace.SetState(birdToDelete, Placemaker.Life.BirdFlock.State.Leaving);
						birdflockPlace.birds.Remove(birdToDelete);
						birdToDelete.transform.SetParent(birdflockPlace.disabledBirds);
						birdflockPlace.activeBirdsCount -= 1;

						myAmountBirds = birdflockPlace.activeBirdsCount;
						

					}
                }
			}
        }

		public static void ControlBirds(bool choice)
        {
			controlAmountBirds = choice;
        }

		public static void UpdateAmountBirds()
        {
			if (birdflock && controlAmountBirds)
            {
				birdflockPlace.idealBirdCount = (byte)myAmountBirds;
				birdflockPlace.minBirdCount = (byte)(myAmountBirds - 1);
				birdflockPlace.minBirdCount = (byte)(myAmountBirds + 1);
			}
        }

		//Initialize Bird creation by fetching Birds UVs from meshes in BirdFlock and store them in posTexture
		public static bool CreateMultiBirds()
		{
			if (!initialized)
			{
				birdflock = GameObject.Find("BirdFlock");
				MelonLogger.Msg("Create used");

				if (!birdflock)
				{
					return false;
				}
				
				birdflockPlace = birdflock.GetComponent<Placemaker.Life.BirdFlock>();
				if (!birdflockPlace)
				{
					return false;
				}

				flapMesh = birdflockPlace.flapMesh;
				flyMesh = birdflockPlace.flyMesh;
				glideMesh = birdflockPlace.glideMesh;
				halfSitMesh = birdflockPlace.halfSitMesh;
				landMesh = birdflockPlace.landMesh;
				sitMesh = birdflockPlace.sitMesh;
				standMesh = birdflockPlace.standMesh;

				Mesh[] meshArray = new Mesh[] { flapMesh, flyMesh, glideMesh, halfSitMesh, landMesh, sitMesh, standMesh };
				Il2CppSystem.Collections.Generic.List<Vector2> myList = new Il2CppSystem.Collections.Generic.List<Vector2>();	

				for (int i = 0; i < posTextures.Length; i++)
				{
					(Vector2 thisVect, var thisMeshArray) = posTextures[i];

					for (int j = 0; j < meshArray.Length; j++)
					{
                        meshArray[j].GetUVs(0, myList);
                        for (int k = 0; k < myList.Count; k++)
                        {
                            myList.set_Item(k, myList.get_Item(k) + thisVect);
                        }

						thisMeshArray[j] = UnityEngine.Object.Instantiate(meshArray[j]);
						thisMeshArray[j].SetUVs(0, myList);
					}
				}

				initialized = true;
				return true;
			}
			return true;
		}

		//Navigate all gameobjects to get all bird bodies and store them in a list (birds)
		public static void GetBirds()
		{

			//Update PercentBirds if not == 1f
			double sumPercent = 0;
			foreach (double percent in percentBirds)
			{
				sumPercent += percent;
			}

			if (sumPercent < 1f)
			{
				percentBirds[0] += 1f - sumPercent;
				BirdsUI.UpdateBirdPopSliders();
			}

			GameObject[] arrayBody = FindGameObjectsWithName("Body");
			MelonLogger.Msg(arrayBody.Length);

			for (int i = 0; i < arrayBody.Length; i++)
			{
				GameObject mesh0 = arrayBody[i].transform.FindChild("Mesh0").gameObject;
				if (mesh0 == null)
				{
					MelonLogger.Msg("Prob");
					return;
				}
				MeshFilter thisMeshFilter0 = mesh0.GetComponent<MeshFilter>();
				if (thisMeshFilter0 == null)
				{
					MelonLogger.Msg("Prob");
					return;
				}

				if (birdFilters == null || !birdFilters.Contains(thisMeshFilter0))
				{
					MeshFilter thisMeshFilter1 = arrayBody[i].transform.FindChild("Mesh1").gameObject.GetComponent<MeshFilter>();
					int thisIndex = PercentChoose();

					if (birds[thisIndex] == null)
                    {
						birds[thisIndex] = new List<(MeshFilter, MeshFilter)> { };
                    }
					birds[thisIndex].Add((thisMeshFilter0, thisMeshFilter1));

					birdFilters.Add(thisMeshFilter0);
				}
			}
		}

		//Change the UVs of Birds meshes every frame
		public static void UpdateBirds()
		{
			for (int i = 1; i < birds.Length; i++)
			{
				if (birds[i] == null)
                {
					continue;
                }

				foreach ((MeshFilter filter0, MeshFilter filter1) in birds[i])
				{
					if (filter0.gameObject.active == true)
                    {
						if (filter0.sharedMesh == flapMesh)
						{
							filter0.sharedMesh = posTextures[i - 1].Item2[0];
						}
						else if (filter0.sharedMesh == flyMesh)
						{
							filter0.sharedMesh = posTextures[i - 1].Item2[1];
						}
						else if (filter0.sharedMesh == glideMesh)
						{
							filter0.sharedMesh = posTextures[i - 1].Item2[2];
						}
						else if (filter0.sharedMesh == halfSitMesh)
						{
							filter0.sharedMesh = posTextures[i - 1].Item2[3];
						}
						else if (filter0.sharedMesh == landMesh)
						{
							filter0.sharedMesh = posTextures[i - 1].Item2[4];
						}
						else if (filter0.sharedMesh == sitMesh)
						{
							filter0.sharedMesh = posTextures[i - 1].Item2[5];
						}
						else if (filter0.sharedMesh == standMesh)
						{
							filter0.sharedMesh = posTextures[i - 1].Item2[6];
						}
					}


					if (filter1.sharedMesh == flapMesh)
					{
						filter1.sharedMesh = posTextures[i - 1].Item2[0];
					}
					else if (filter1.sharedMesh == flyMesh)
					{
						filter1.sharedMesh = posTextures[i - 1].Item2[1];
					}
					else if (filter1.sharedMesh == glideMesh)
					{
						filter1.sharedMesh = posTextures[i - 1].Item2[2];
					}
					else if (filter1.sharedMesh == halfSitMesh)
					{
						filter1.sharedMesh = posTextures[i - 1].Item2[3];
					}
					else if (filter1.sharedMesh == landMesh)
					{
						filter1.sharedMesh = posTextures[i - 1].Item2[4];
					}
					else if (filter1.sharedMesh == sitMesh)
					{
						filter1.sharedMesh = posTextures[i - 1].Item2[5];
					}
					else if (filter1.sharedMesh == standMesh)
					{
						filter1.sharedMesh = posTextures[i - 1].Item2[6];
					}

				}
			}
		}

		private static GameObject[] FindGameObjectsWithName(string name)
		{
			var allGameObjects = GameObject.FindObjectsOfType<GameObject>();
			int a = allGameObjects.Length;
			//MelonLogger.Msg(a);
			GameObject[] arr = new GameObject[a];
			int FluentNumber = 0;
			for (int i = 0; i < a; i++)
			{
				if (allGameObjects[i].name == name)
				{
					arr[FluentNumber] = allGameObjects[i];
					FluentNumber++;
				}
			}
			Array.Resize(ref arr, FluentNumber);
			return arr;
		}

		private static int PercentChoose()
		{
			double thisRand = rand.NextDouble();
			double percent = 0f;
			for (int i = 0; i < percentBirds.Length; i++)
			{
				if (thisRand >= percent && thisRand <= percent + percentBirds[i])
				{
					return i;
				}
				else
				{
					percent += percentBirds[i];
				}
			}
			return 0;
		}

		public static void ResetBirds()
		{

			foreach (var bird in birds)
			{
				if (bird != null)
                {
					//Bring back original texture
					foreach ((MeshFilter filter0, MeshFilter filter1) in bird)
					{
						if (filter0.sharedMesh.name.Contains("Bird_Flapping"))
						{
							filter0.sharedMesh = flapMesh;
						}
						else if (filter0.sharedMesh.name.Contains("Bird_Flying"))
						{
							filter0.sharedMesh = flyMesh;
						}
						else if (filter0.sharedMesh.name.Contains("Bird_Gliding"))
						{
							filter0.sharedMesh = glideMesh;
						}
						else if (filter0.sharedMesh.name.Contains("Bird_Stop_HalfStanding"))
						{
							filter0.sharedMesh = halfSitMesh;
						}
						else if (filter0.sharedMesh.name.Contains("Bird_Landing"))
						{
							filter0.sharedMesh = landMesh;
						}
						else if (filter0.sharedMesh.name.Contains("Bird_Stop_Sitting"))
						{
							filter0.sharedMesh = sitMesh;
						}
						else if (filter0.sharedMesh.name.Contains("Bird_Stop_Standing"))
						{
							filter0.sharedMesh = standMesh;
						}


						if (filter1.sharedMesh.name.Contains("Bird_Flapping"))
						{
							filter1.sharedMesh = flapMesh;
						}
						else if (filter1.sharedMesh.name.Contains("Bird_Flying"))
						{
							filter1.sharedMesh = flyMesh;
						}
						else if (filter1.sharedMesh.name.Contains("Bird_Gliding"))
						{
							filter1.sharedMesh = glideMesh;
						}
						else if (filter1.sharedMesh.name.Contains("Bird_Stop_HalfStanding"))
						{
							filter1.sharedMesh = halfSitMesh;
						}
						else if (filter1.sharedMesh.name.Contains("Bird_Landing"))
						{
							filter1.sharedMesh = landMesh;
						}
						else if (filter1.sharedMesh.name.Contains("Bird_Stop_Sitting"))
						{
							filter1.sharedMesh = sitMesh;
						}
						else if (filter1.sharedMesh.name.Contains("Bird_Stop_Standing"))
						{
							filter1.sharedMesh = standMesh;
						}

					}


					//Cleaning the store place
					bird.Clear();
				}
			}
			
			birdFilters.Clear();
		}

		public static void BoxBirdCreate()
        {
			//Test Box Collider
			foreach (GameObject mesh1 in FindGameObjectsWithName("Mesh1"))
			{
				MelonLogger.Msg("OK");
				MeshCollider compFound = mesh1.GetComponent<MeshCollider>();
				MelonLogger.Msg("OK1");

				if (compFound == null)
                {
					MelonLogger.Msg("OK2");
					MeshCollider meshc = mesh1.AddComponent<MeshCollider>();
					//Rigidbody rigid = mesh1.AddComponent<Rigidbody>();
					//rigid.isKinematic = true;
					meshc.sharedMesh = standMesh;
					//box.size = renderer.bounds.size;
					//mesh1.layer = 21;
					MelonLogger.Msg("OK3");
				}
			}
		}

		public static void ColorBirds()
        {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit raycastHit, 1E+07f))
			{
				GameObject thisHit = raycastHit.collider.gameObject;
				if (thisHit.name == "Mesh1")
				{
					chosenBird = thisHit.transform.parent.GetComponentInParent<Placemaker.Life.Bird>();
					MeshFilter chosenMeshF1 = thisHit.GetComponent<MeshFilter>();
					MeshFilter chosenMeshF0 = thisHit.transform.parent.FindChild("Mesh0").GetComponent<MeshFilter>();
					if (chosenBird)
					{
						//Debug.DrawRay(ray.origin, ray.direction * raycastHit.distance, Color.yellow);
						//MelonLogger.Msg("Chosen" + chosenBird.transform.position.ToString());
						//birdflockPlace.UprootBird(chosenBird, 1f);
						chosenBird = null;

						(MeshFilter, MeshFilter) birdToMove = (null, null);
						int indexToMove = 100;
						bool found = false;

						//Browse birds to find the color of chosenbird

						//Case no birds are colored yet

						for (int i = 0; i < birds.Length; i++)
						{
							if (birds[i] == null)
							{
								continue;
							}

							foreach (var item in birds[i])
							{
								(MeshFilter filter0, MeshFilter filter1) = item;
								if (filter0 == chosenMeshF0 && filter1 == chosenMeshF1)
								{
									birdToMove = item;
									indexToMove = i;
									found = true;
									break;
								}
							}
							if (found)
							{
								break;
							}
						}

						if (found == false || birdFilters.Count == 0)
						{
							if (birds[1] == null)
							{
								birds[1] = new List<(MeshFilter, MeshFilter)> { };
							}
							birds[1].Add((chosenMeshF0, chosenMeshF1));
							birdFilters.Add(chosenMeshF0);
							indexToMove = 0;
						}

						//Changing Bird position
						else if (birdToMove != (null, null) && indexToMove != 100)
						{
							birds[indexToMove].Remove(birdToMove);
							if (indexToMove + 1 == birds.Length)
							{
								if (birds[0] == null)
								{
									birds[0] = new List<(MeshFilter, MeshFilter)> { };
								}
								birds[0].Add(birdToMove);
							}
							else
							{
								if (birds[indexToMove + 1] == null)
								{
									birds[indexToMove + 1] = new List<(MeshFilter, MeshFilter)> { };
								}
								birds[indexToMove + 1].Add(birdToMove);
							}
						}

						//Updating colors
						int thisLength = birds.Length;
						if (chosenMeshF1.sharedMesh.name.Contains("Bird_Flapping"))
						{
							chosenMeshF1.sharedMesh = indexToMove == 0 ? posTextures[0].Item2[0] : indexToMove + 1 == thisLength ? flapMesh : posTextures[indexToMove].Item2[0];
						}
						else if (chosenMeshF1.sharedMesh.name.Contains("Bird_Flying"))
						{
							chosenMeshF1.sharedMesh = indexToMove == 0 ? posTextures[0].Item2[1] : indexToMove + 1 == thisLength ? flyMesh : posTextures[indexToMove].Item2[1];
						}
						else if (chosenMeshF1.sharedMesh.name.Contains("Bird_Gliding"))
						{
							chosenMeshF1.sharedMesh = indexToMove == 0 ? posTextures[0].Item2[2] : indexToMove + 1 == thisLength ? glideMesh : posTextures[indexToMove].Item2[2];
						}
						else if (chosenMeshF1.sharedMesh.name.Contains("Bird_Stop_HalfStanding"))
						{
							chosenMeshF1.sharedMesh = indexToMove == 0 ? posTextures[0].Item2[3] : indexToMove + 1 == thisLength ? halfSitMesh : posTextures[indexToMove].Item2[3];
						}
						else if (chosenMeshF1.sharedMesh.name.Contains("Bird_Landing"))
						{
							chosenMeshF1.sharedMesh = indexToMove == 0 ? posTextures[0].Item2[4] : indexToMove + 1 == thisLength ? landMesh : posTextures[indexToMove].Item2[4];
						}
						else if (chosenMeshF1.sharedMesh.name.Contains("Bird_Stop_Sitting"))
						{
							chosenMeshF1.sharedMesh = indexToMove == 0 ? posTextures[0].Item2[5] : indexToMove + 1 == thisLength ? sitMesh : posTextures[indexToMove].Item2[5];
						}
						else if (chosenMeshF1.sharedMesh.name.Contains("Bird_Stop_Standing"))
						{
							chosenMeshF1.sharedMesh = indexToMove == 0 ? posTextures[0].Item2[6] : indexToMove + 1 == thisLength ? standMesh : posTextures[indexToMove].Item2[6];
						}
					}
				}

			}	

		}

		public static void HuntBirds()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit raycastHit, 1E+07f))
			{
				GameObject thisHit = raycastHit.collider.gameObject;
				if (thisHit.name == "Mesh1")
                {
					chosenBird = thisHit.transform.parent.GetComponentInParent<Placemaker.Life.Bird>();
					birdflockPlace.UprootBird(chosenBird, 1f);
					chosenBird = null;
				}
				
			}

		}


		public override void OnApplicationStart()
		{
			
		}

		public override void OnUpdate()
		{
			//Update amount of birds
			UpdateAmountBirds();

			//Hunt Birds
			if (Input.GetKeyDown(UpRootBirdKey))
            {
				if (!colliderUpToDate)
                {
					BoxBirdCreate();
					colliderUpToDate = true;
                }
				HuntBirds();
            }

			//Hunt All Birds
			if (Input.GetKeyDown(UpRootAllBirdsKey))
			{
				birdflockPlace.UprootAllBirds();				
			}


			//Other
			if (Input.GetKeyDown(ColorBirdKey))
            {
				if (!colliderUpToDate)
				{
					BoxBirdCreate();
					colliderUpToDate = true;
				}
				NPressed = true;

			}
					


			else if (Input.GetKeyUp(ColorBirdKey))
            {
				NPressed = false;
            }
		}

	}
}

