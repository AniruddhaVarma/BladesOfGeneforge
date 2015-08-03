using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
 * The floorsets (and possibly wallsets) are replaceable and are numbered the same in the
 * ScenData file, ie a floorset points to one of several inter-changeable sprite sheets
 * such as 'Snow' and 'Grass'
 * 
 */
namespace DMCGeneforgeMpTiles
{
    class cFloor
    {
        public static cFloor[] Floors;

        //default values to save definition code (changes to reflect most recent definition)
        public static int _iGraphicTemplate;    // fl_graphic_template
        public static int _iRow;                // fl_graphic_sheet  (begins 0)
        public static int _iCol;                // fl_which_icon     (begins 0)

        
        int iGraphicTemplate;

        public void setGraphicTemplate(int i)
        {
            iGraphicTemplate = _iGraphicTemplate = i;
        }

        int iRow;

        public void setRow(int i)
        {
            iRow = _iRow = i;
        }
        
        int iCol;

        public void setCol(int i)
        {
            iCol = _iCol = i;
        }

        cFloor()
        {
            //Init with the default(i.e. static) values. If values are different than this, can be changed later
            iGraphicTemplate = _iGraphicTemplate;
            iRow = _iRow;
            iCol = _iCol;
        }
        cFloor(ref cFloor old)
        {
            iGraphicTemplate = _iGraphicTemplate = old.iGraphicTemplate;
            iRow = _iRow = old.iRow;
            iCol = _iCol = old.iCol;
        }

        public static bool RetrieveRowAndCol(int iFloorNum,ref int iRow,ref int iCol)
        {
            if (iFloorNum > 28) return false;
            iRow = Floors[iFloorNum].iRow;
            iCol = Floors[iFloorNum].iCol;
            return true;
        }

        /// <summary>
        /// Define the array of all floor types
        /// </summary>
        public static void InitializeFloors()
        {
            //define the default values to be reused
            cFloor._iGraphicTemplate = 0;
            cFloor._iRow = 0;
            cFloor._iCol = 0;

            Floors = new cFloor[100];
            //6 basic grounds
            Floors[0] = new cFloor(); //Ground
            Floors[1] = new cFloor(); Floors[1].setCol(1);
            Floors[2] = new cFloor(); Floors[2].setCol(2);
            Floors[3] = new cFloor(); Floors[3].setCol(3);
            Floors[4] = new cFloor(); Floors[4].setCol(4);
            Floors[5] = new cFloor(); Floors[5].setCol(5);
            //Grass/Water
            Floors[6] = new cFloor(); Floors[6].setCol(6); //Water
            Floors[7] = new cFloor(); Floors[7].setCol(7); //DeepWater
            Floors[8] = new cFloor(); Floors[8].setCol(0); //Shore
             Floors[8].setRow(1);
            Floors[9] = new cFloor(); Floors[9].setCol(1);
            Floors[10] = new cFloor(); Floors[10].setCol(2);
            Floors[11] = new cFloor(); Floors[11].setCol(3);
            Floors[12] = new cFloor(); Floors[12].setCol(4);
            Floors[13] = new cFloor(); Floors[13].setCol(5);
            Floors[14] = new cFloor(); Floors[14].setCol(6);
            Floors[15] = new cFloor(); Floors[15].setCol(0);
             Floors[15].setRow(2);
            Floors[16] = new cFloor(); Floors[16].setCol(1);
            Floors[17] = new cFloor(); Floors[17].setCol(2);
            Floors[18] = new cFloor(); Floors[18].setCol(3);
            Floors[19] = new cFloor(); Floors[19].setCol(4);
            Floors[20] = new cFloor(ref Floors[7]); // Water w Island
             Floors[20].setRow(2);
             Floors[20].setCol(5);
            Floors[21] = new cFloor(); Floors[21].setCol(6); // Water w Rock
            //grass with misc features
            Floors[25] = new cFloor(ref Floors[0]); //Boulder
             Floors[25].setRow(3);
             Floors[25].setCol(0);
            Floors[26] = new cFloor(); Floors[26].setCol(1);
            Floors[27] = new cFloor(); Floors[27].setCol(2); //Huge boulder blocks whole space
            Floors[28] = new cFloor(ref Floors[0]);//Stone Path
             Floors[28].setRow(3);
             Floors[28].setCol(3);
            Floors[29] = new cFloor(ref Floors[0]); // Crops
             Floors[29].setCol(4);
             Floors[29].setRow(3);
            Floors[30] = new cFloor(ref Floors[27]); // Pool
             Floors[30].setRow(3);
             Floors[30].setCol(5);
            Floors[31] = new cFloor(ref Floors[0]); // Rocky Ground
             Floors[31].setRow(3);
             Floors[31].setCol(6);
            Floors[32] = new cFloor(); Floors[32].setCol(7); // Bony Ground
            Floors[33] = new cFloor(ref Floors[0]); // Crops
             Floors[33].setRow(7);
             Floors[33].setCol(7);
            Floors[34] = new cFloor();
             Floors[34].setRow(8);
             Floors[34].setCol(7);
            //Walkway and grass/walkway
            Floors[35] = new cFloor(ref Floors[0]); // Walkway
             Floors[35].setGraphicTemplate(25);
             Floors[35].setRow(4);
             Floors[35].setCol(5);
            Floors[36] = new cFloor(); Floors[36].setCol(6); //Dirty Walkway
            Floors[37] = new cFloor();//Walkway Edge
             Floors[37].setRow(4);
             Floors[37].setCol(0);










        }
    }

}
