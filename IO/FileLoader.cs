using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Siege.IO {
    public class FileLoader {
        //The location of the file that is handled by this FileLoader
        string pathLocation;

        public FileLoader(string pathLocation) {
            this.pathLocation = pathLocation;
        }

        /// <summary>
        /// Loads a file and returns a file data node containing the data within
        /// the file.
        /// 
        /// Data is stored in KeyValuePairs, if the data is in the root node
        /// the string will be the key and the Object will be the value which
        /// requires casting.
        /// 
        /// If the data is in a child node the Object will be another Dictionary
        /// containing more KeyValuePairs, nodes can recurse infinately to store
        /// data in a powerful system. 
        /// 
        /// </summary>
        /// <returns>A file data node.</returns>
        public FileNode Load() {
            StreamReader reader;

            // Attempt to creaate the file reader, if an exception is thrown the file
            // most likely doesent exist so create it and then attempt to create the
            // file reader again.
            try {
                reader = new StreamReader(pathLocation);
            } catch (Exception ex) {
                Directory.CreateDirectory(Path.GetDirectoryName(pathLocation));
                File.Create(pathLocation).Dispose();
                reader = new StreamReader(pathLocation);
            }

            // Stores raw data for FileNode(s) which can then be converted to its object form.
            //      Node Level        Node path     Data 
            Dictionary<int, Dictionary<string, List<string>>> data = new Dictionary<int, Dictionary<string, List<string>>>();
            // Atempt to read and then parse the file into a FileNode. After that, close
            // the file reader and then return the created FileNode.
            try {
                string line = reader.ReadLine();

                // Set the current NodeLevel to be 0 and the Path to be an empty string as we are currently at the
                // root node.
                int currentNodeLevel = 0;
                string currentNodePath = "";

                //Check the line isn't null. 
                while (line != null) {
                    //If the line ends with '{' it means we are in a higher node level, increment the current level
                    //and update the node path to correspond to the point we are currently at.
                    if (line.EndsWith("{")) {
                        currentNodeLevel++;
                        if (!data.ContainsKey(currentNodeLevel) && currentNodeLevel != 0) {
                            Dictionary<string, List<string>> plainDataNode = new Dictionary<string, List<string>>();
                            data.Add(currentNodeLevel, plainDataNode);
                        }

                        if (!(currentNodePath == "")) currentNodePath += ".";
                        currentNodePath += line.Split('{')[0];

                        // Create an empty data node for the newly found child node and add it to the Data directory.
                        Dictionary<string, List<string>> nodeLevelData;
                        data.TryGetValue(currentNodeLevel, out nodeLevelData);
                        if (!nodeLevelData.ContainsKey(currentNodePath)) {
                            nodeLevelData.Add(currentNodePath, new List<string>());
                        }
                    }

                    // If the line ends with '}' it means we are in a lower node level, decrement the current level
                    // and update the node path to correspond to the point we are currently at.
                    else if (line.EndsWith("}")) {
                        currentNodeLevel--;

                        if (!(currentNodePath.Contains("."))) {
                            currentNodePath = "";
                        } else {
                            currentNodePath = currentNodePath.Substring(0, currentNodePath.LastIndexOf('.'));
                        }
                    } else {
                        // We made it this far, this means the line is a key/value - save it in the current node level and path.
                        if (!data.ContainsKey(currentNodeLevel)) data.Add(currentNodeLevel, new Dictionary<string, List<string>>());

                        // Get the Data list for the current Node Level and path, if it doesn't exist, create it.
                        Dictionary<string, List<string>> currentOperatingLevelNodeData;
                        data.TryGetValue(currentNodeLevel, out currentOperatingLevelNodeData); //We can be confident this is not null as we just created it if this was the case.
                        if (!currentOperatingLevelNodeData.ContainsKey(currentNodePath)) currentOperatingLevelNodeData.Add(currentNodePath, new List<string>());

                        // At this point we have loaded the relavent storage list, save the data item and set the dictionary back
                        // to the primary dictionary.
                        List<string> currentOperatingNodeData;
                        currentOperatingLevelNodeData.TryGetValue(currentNodePath, out currentOperatingNodeData);
                        currentOperatingNodeData.Add(line);
                        currentOperatingLevelNodeData[currentNodePath] = currentOperatingNodeData;
                        data[currentNodeLevel] = currentOperatingLevelNodeData;
                    }

                    //Read the next line.
                    line = reader.ReadLine(); 
                }
            } catch (Exception ex) {
                Console.WriteLine("An error occured reading the file: {0}", pathLocation);
            } finally {
                reader.Close();
            }

            // Create the FileNode from the Data which has been processed into the directory
            FileNode rootNode = new FileNode();

            // Iteraate through each level and path of Data in the directory.
            foreach (KeyValuePair<int, Dictionary<string, List<string>>> levelDataPair in data) {
                foreach (KeyValuePair<string, List<string>> pathDataPair in levelDataPair.Value) {
                    // If the level is 0 that means that it is a singleton list with a value, parse the only value and
                    // set it back to the root node.
                    if (levelDataPair.Key == 0) {
                        string rootData = pathDataPair.Value.First();
                        rootNode.SetValue(rootData.Split('=')[0], rootData.Split('=')[1]);
                        continue;
                    }

                    // If we get this far the data is in a child node, load the data into a FileNode and then set it back to the
                    // appropriate path.
                    FileNode node = LoadNodeFromData(pathDataPair.Value);
                    if (levelDataPair.Key == 1) {
                        rootNode.SetValue(pathDataPair.Key, node);
                        continue;
                    }

                    // If the level is > 1 that means it needs to be set back to a child node, find the node first and then set it
                    // back. We can assume the node exists as they are created in level order.
                    FileNode wanted = rootNode;
                    string[] path = pathDataPair.Key.Split('.');
                    for (int i = 0; i < levelDataPair.Key - 1; i++) {
                        wanted = wanted.GetNode(path[i]);
                    }
                    wanted.SetValue(pathDataPair.Key.Substring(pathDataPair.Key.LastIndexOf('.') + 1), node);
                }
            }

            // Return the created FileNode.
            return rootNode;
        }

        /// <summary>
        /// Loads data from a list of data and forms a FileNode.
        /// </summary>
        /// <param name="dataList">The data to use in order to form the node.</param>
        /// <returns>The newly created FileNode</returns>
        public FileNode LoadNodeFromData(List<string> dataList) {
            FileNode node = new FileNode();

            //Iterate through each line of data and add it to the file node.
            dataList.ForEach(data => {
                string[] splitLine = data.Split('=');

                //If the data isnt two parts, its not as expected.
                if (splitLine.Length == 2) {
                    node.SetValue(splitLine[0], splitLine[1]);
                } else {
                    Console.WriteLine("Attempted to load node data '" + data + "' but it was not in the expected format.");
                }
            });

            return node;
        }

        /// <summary>
        /// Saves the contents of a FileNode to this Loaders pathLocation.
        /// </summary>
        /// <param name="node">The FileNode to write.</param>
        public void Save(FileNode node) {
            StreamWriter writer = new StreamWriter(pathLocation); //Create a new stream writer to write to the file.

            // Get the data to write from the FileNode. After that, iterate through the retrieved data and try to 
            // write each line to the file. Finally, close the writer.
            List<string> data = node.GetFileWriteableData();
            try {
                foreach (string line in data) {
                    writer.WriteLine(line);
                }
            } catch (Exception ex) {
                Console.WriteLine("An error occured writing to the file: {0}", pathLocation);
            } finally {
                writer.Close();
            }
        }
    }
}
