 using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ChessBehaviour : MonoBehaviour {
    private enum ChessPieceType {
        Knight,
        Queen,
        King,
        Bishop,
        Rook
    }
    private enum BoardState {
        Empty,
        Covered,
        Filled
    }
    private class ChessPiece {
        public int xCoord, yCoord;
        public ChessPieceType piece;

        public static bool getWhite(int x, int y) {
            return x % 2 != y % 2;
        }

        public ChessPiece(int x, int y, ChessPieceType p, ref BoardState[,] board) {
            xCoord = x;
            yCoord = y;
            piece = p;
            board[x, y] = BoardState.Filled;
        }
        public static void getRookTouchable(int x, int y, ref BoardState[,] myList, bool tempKing = false) {


            for (int i = x + 1; i < 6; i++) {
                if (myList[i, y] == BoardState.Empty) {
                    myList[i, y] = BoardState.Covered;
                } else if (myList[i, y] == BoardState.Filled) {
                    break;
                }
                if (tempKing) break;

            }

            for (int i = x - 1; i >= 0; i--) {
                if (myList[i, y] == BoardState.Empty) {
                    myList[i, y] = BoardState.Covered;
                } else if (myList[i, y] == BoardState.Filled) {
                    break;
                }
                if (tempKing) break;

            }

            for (int i = y + 1; i < 6; i++) {
                if (myList[x, i] == BoardState.Empty) {
                    myList[x, i] = BoardState.Covered;
                } else if (myList[x, i] == BoardState.Filled) {
                    break;
                }
                if (tempKing) break;

            }

            for (int i = y - 1; i >= 0; i--) {
                if (myList[x, i] == BoardState.Empty) {
                    myList[x, i] = BoardState.Covered;
                } else if (myList[x, i] == BoardState.Filled) {
                    break;
                }
                if (tempKing) break;

            }
        }

        public static void getBishopTouchable(int x, int y, ref BoardState[,] myList, bool tempKing = false) {


            for (int i = x + 1, j = y + 1; j < 6 && i < 6; i++, j++) {
                if (myList[i, j] == BoardState.Empty) {
                    myList[i, j] = BoardState.Covered;
                } else if (myList[i, j] == BoardState.Filled) {
                    break;
                }
                if (tempKing) break;

            }

            for (int i = x - 1, j = y + 1; j < 6 && i >= 0; i--, j++) {
                if (myList[i, j] == BoardState.Empty) {
                    myList[i, j] = BoardState.Covered;
                } else if (myList[i, j] == BoardState.Filled) {
                    break;
                }
                if (tempKing) break;

            }

            for (int i = x + 1, j = y - 1; j >= 0 && i < 6; i++, j--) {
                if (myList[i, j] == BoardState.Empty) {
                    myList[i, j] = BoardState.Covered;
                } else if (myList[i, j] == BoardState.Filled) {
                    break;
                }
                if (tempKing) break;

            }

            for (int i = x - 1, j = y - 1; j >= 0 && i >= 0; i--, j--) {
                if (myList[i, j] == BoardState.Empty) {
                    myList[i, j] = BoardState.Covered;
                } else if (myList[i, j] == BoardState.Filled) {
                    break;
                }
                if (tempKing) break;

            }
        }


        public static void getQueenTouchable(int x, int y, ref BoardState[,] myList) {
            getRookTouchable(x, y, ref myList);
            getBishopTouchable(x, y, ref myList);
        }

        public static void getKnightTouchable(int x, int y, ref BoardState[,] myList) {
            int newX, newY;
            newX = x + 2;
            newY = y + 1;
            if (newX >= 0 && newX < 6 && newY < 6 && newY >= 0 && myList[newX, newY] == BoardState.Empty) {
                myList[newX, newY] = BoardState.Covered;
            }
            newX = x - 2;
            newY = y + 1;
            if (newX >= 0 && newX < 6 && newY < 6 && newY >= 0 && myList[newX, newY] == BoardState.Empty) {
                myList[newX, newY] = BoardState.Covered;
            }
            newX = x + 2;
            newY = y - 1;
            if (newX >= 0 && newX < 6 && newY < 6 && newY >= 0 && myList[newX, newY] == BoardState.Empty) {
                myList[newX, newY] = BoardState.Covered;
            }
            newX = x - 2;
            newY = y - 1;
            if (newX >= 0 && newX < 6 && newY < 6 && newY >= 0 && myList[newX, newY] == BoardState.Empty) {
                myList[newX, newY] = BoardState.Covered;
            }
            newX = x + 1;
            newY = y + 2;
            if (newX >= 0 && newX < 6 && newY < 6 && newY >= 0 && myList[newX, newY] == BoardState.Empty) {
                myList[newX, newY] = BoardState.Covered;
            }
            newX = x - 1;
            newY = y + 2;
            if (newX >= 0 && newX < 6 && newY < 6 && newY >= 0 && myList[newX, newY] == BoardState.Empty) {
                myList[newX, newY] = BoardState.Covered;
            }
            newX = x + 1;
            newY = y - 2;
            if (newX >= 0 && newX < 6 && newY < 6 && newY >= 0 && myList[newX, newY] == BoardState.Empty) {
                myList[newX, newY] = BoardState.Covered;
            }
            newX = x - 1;
            newY = y - 2;
            if (newX >= 0 && newX < 6 && newY < 6 && newY >= 0 && myList[newX, newY] == BoardState.Empty) {
                myList[newX, newY] = BoardState.Covered;
            }

        }

        public static void getKingTouchable(int x, int y, ref BoardState[,] myList) {
            getRookTouchable(x, y, ref myList, true);
            getBishopTouchable(x, y, ref myList, true);
        }


        public void getTouchable(ref BoardState[,] myList) {
            switch (piece) {
                case ChessPieceType.Queen:
                    getQueenTouchable(xCoord, yCoord, ref myList);
                    break;
                case ChessPieceType.King:
                    getKingTouchable(xCoord, yCoord, ref myList);
                    break;
                case ChessPieceType.Bishop:
                    getBishopTouchable(xCoord, yCoord, ref myList);
                    break;
                case ChessPieceType.Knight:
                    getKnightTouchable(xCoord, yCoord, ref myList);
                    break;
                case ChessPieceType.Rook:
                    getRookTouchable(xCoord, yCoord, ref myList);
                    break;
            }
        }

    }

    public KMSelectable[] bottomButtons;
    public KMSelectable[] topButtons;
    public KMBombInfo mBombInfo;
    public KMBombModule mBombModule;
    public KMAudio mAudio;
    public TextMesh letterMesh;
    public TextMesh numberMesh;
    public MeshRenderer[] lightsArray;


    [HideInInspector]
    public bool isSolved = false;
    private int previousNumberSelected;
    private bool isLetterPressed;
    private char letterSelected;
    private string[][] generatedPairs;
    private string[] indexSelected;

    public static bool TestRand(ref string[][] generatedPairs, int Sign) {
        try {
            string[] RandPieces = new string[6];
            string TestString = "";
            int ji = 0;
            while (ji < 6) {
                string temp = (char)(Random.Range(0, 6) + 'a') + "" + (Random.Range(0, 6) + 1);
                if (!TestString.Contains(temp)) { 
                    RandPieces[ji] = temp;
                    TestString += temp;
                    ji++;
                }
            }
            BoardState[,] chessBoard = new BoardState[6, 6];
            ChessPiece[] myPieces;
            for (int i = 0; i < 6; i++) {
                for (int j = 0; j < 6; j++) {
                    chessBoard[i, j] = BoardState.Empty;
                }
            }

            myPieces = new ChessPiece[6];
            myPieces[1] = new ChessPiece((RandPieces[1][0]) - 'a', (RandPieces[1][1]) - '1', (Sign == 0 ? ChessPieceType.Knight : ChessPieceType.Rook), ref chessBoard);
            myPieces[3] = new ChessPiece(RandPieces[3][0] - 'a', RandPieces[3][1] - '1', ChessPieceType.Rook, ref chessBoard);
            if (ChessPiece.getWhite(RandPieces[4][0] - 'a', RandPieces[4][1] - '1')) {
                myPieces[4] = new ChessPiece(RandPieces[4][0] - 'a', RandPieces[4][1] - '1', ChessPieceType.Queen, ref chessBoard);
                myPieces[0] = new ChessPiece(RandPieces[0][0] - 'a', RandPieces[0][1] - '1', ChessPieceType.King, ref chessBoard);
            } else {
                myPieces[4] = new ChessPiece(RandPieces[4][0] - 'a', RandPieces[4][1] - '1', ChessPieceType.Rook, ref chessBoard);
                myPieces[0] = new ChessPiece(RandPieces[0][0] - 'a', RandPieces[0][1] - '1', ChessPieceType.Bishop, ref chessBoard);
            }

            if (Sign == 1 || myPieces[4].piece == ChessPieceType.Rook) {
                myPieces[2] = new ChessPiece(RandPieces[2][0] - 'a', RandPieces[2][1] - '1', ChessPieceType.King, ref chessBoard);
            } else {
                myPieces[2] = new ChessPiece(RandPieces[2][0] - 'a', RandPieces[2][1] - '1', ChessPieceType.Queen, ref chessBoard);
            }

            bool hasQueen = false;
            bool hasKnight = false;
            for (int i = 0; i < 5; i++) {
                if (myPieces[i].piece == ChessPieceType.Queen) {
                    hasQueen = true;
                }
                if (myPieces[i].piece == ChessPieceType.Knight) {
                    hasKnight = true;
                }
            }
            if (!hasQueen) {
                myPieces[5] = new ChessPiece(RandPieces[5][0] - 'a', RandPieces[5][1] - '1', ChessPieceType.Queen, ref chessBoard);
            } else if (!hasKnight) {
                myPieces[5] = new ChessPiece(RandPieces[5][0] - 'a', RandPieces[5][1] - '1', ChessPieceType.Knight, ref chessBoard);
            } else {
                myPieces[5] = new ChessPiece(RandPieces[5][0] - 'a', RandPieces[5][1] - '1', ChessPieceType.Bishop, ref chessBoard);
            }
            foreach (ChessPiece i in myPieces) {
                i.getTouchable(ref chessBoard);
            }

            int empty = 0;
            List<string> emptyArr = new List<string>();
            for (int i = 5; i >= 0 && empty < 2; i--) {

                for (int j = 0; j < 6 && empty < 2; j++) {
                    if (chessBoard[j, i] == BoardState.Empty) {
                        empty++;
                        emptyArr.Add((char)(j + 'a') + "" + (i + 1));
                    }

                }
            }
            if (empty == 1) {
                for (int i = 0; i < 6; i++) {
                    generatedPairs[Sign][i] = RandPieces[i];
                }
                generatedPairs[Sign][6] = emptyArr[0];
                return true;

            }
        }catch(UnityException e) {
            Debug.Log(e.Message);
            Debug.Log("TestRand Exception");
        }
        return false;
    }
    void SetSolveIndex() {
        indexSelected = new string[7];
        string str = "abcde0";
        if (mBombInfo != null) {
            List<string> list = mBombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_SERIAL_NUMBER, null);
            if (list.Count > 0)
                str = JsonConvert.DeserializeObject<Dictionary<string, string>>(list[0])["serial"];
        }
        indexSelected = generatedPairs[str[5] % 2];
        string MyDebugString = "";
        foreach(string s in indexSelected) {
            MyDebugString += s + ", ";
        }
        Debug.Log("[Chess]Selected Solution: " + MyDebugString);
    }

    void CheckSolve(int num) {
        for(int i = 0; i < lightsArray.Length; i++) {
            lightsArray[i].material.color = new Color(0, 0, 0);
        }
        if (letterSelected.ToString() + (num + 1).ToString() == indexSelected[6]) {
            mBombModule.HandlePass();
            isSolved = true;
            DisplayCoords(-1);
        } else {
            mBombModule.HandleStrike();
            DisplayCoords(previousNumberSelected);
        }
        isLetterPressed = false;
    }

    void DisplayCoords(int index) {
        if(index != previousNumberSelected)
            lightsArray[previousNumberSelected].material.color = new Color(0, 0, 0);

        if (index != -1) {
            letterMesh.text = indexSelected[index][0].ToString();
            numberMesh.text = indexSelected[index][1].ToString();
            lightsArray[index].material.color = new Color(0, 1, 0);
            previousNumberSelected = index;
        }else {
            letterMesh.text = "";
            numberMesh.text = "";
        }
    }

    void HandleLetters(char letter) {
        if(!isSolved)
            if (!isLetterPressed) {
                for (int i = 0; i < lightsArray.Length; i++) {
                    lightsArray[i].material.color = new Color(1, 0, 0);
                }
                isLetterPressed = true;
                 letterSelected = letter;
            }
    }

    void HandleNumbers(int num) {
        if(!isSolved)
            if (!isLetterPressed) {
                DisplayCoords(num);
            } else {
                Debug.Log("[Chess]Entered answer: " + letterSelected.ToString() + (num + 1).ToString());
                CheckSolve(num);
            }
    }


    void Start() {
        generatedPairs = new string[2][];
        for(int i = 0; i < 2; i++) {
            generatedPairs[i] = new string[7];
        }
        for(int i = 0; i < 2; i++) {
            for (int j = 0; !TestRand(ref generatedPairs, i) && j < 2500; j++) ;
        }
        isLetterPressed = false;
        for(int i = 0; i < bottomButtons.Length; i++) {
            var temp = i;
            bottomButtons[i].OnInteract += delegate () { mAudio.HandlePlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, bottomButtons[temp].transform); HandleNumbers(temp); return false; };
        }

        for(int i = 0; i < topButtons.Length; i++) {
            var temp = i;
            topButtons[i].OnInteract += delegate () { mAudio.HandlePlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, topButtons[temp].transform); HandleLetters((char)('a' + temp)); return false; };
        }
        mBombModule.OnActivate += OnActivate;
    }
    void OnActivate() {
        SetSolveIndex();
        DisplayCoords(0);
    }

}