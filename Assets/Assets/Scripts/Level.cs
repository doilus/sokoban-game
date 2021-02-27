using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public List<List<Field>> levelLayout = new List<List<Field>>(); //co gdzie się znajduje - Field - opisuje pole

    public int width;
    public int height;

    public List<Vector2Int> targets; // lista współrzęnych, na których są pola docelowe

    public Level() { //jeżeli bez żadnych argumentow to poziom demo; przed nap
        string demoLevel =
@"######
#....#
#@.$_#
#....#
######";
        CreateFromString(demoLevel);
    }

    public Level(string s) {

        TextAsset textAsset = (TextAsset)Resources.Load(s);
        if (!textAsset)
        {
            Debug.Log("Nie znaleziono pliku");
        }
        else { CreateFromString(textAsset.text); }
        
        
    }

    private void CreateFromString(string s) {
        //TODO uzupelnic tworzenie poziomu ***1***
        // # - sciana
        // . - podloga
        // @ - gracz
        // $ - skrzynka
        // _ - miejsce docelowe


        List<Field> rows = new List<Field>(); //wiersz level
        int length = s.Length; 

        //iterowanie po calosci stringa i przy wykryciu \n zmiana na nastepny wiersz
        for(int i = 0; i < length; i++){
            if (s[i] == '#') {
                rows.Add(new Field(FieldType.Wall));
            }
            else if (s[i] == '.') {
                rows.Add(new Field(FieldType.Floor));
            }
            else if(s[i] == '@') {
                rows.Add(new Field(FieldType.Floor, EntityType.Player));
            } 
            else if(s[i] == '$'){
                rows.Add(new Field(FieldType.Floor, EntityType.Box));
            }
            else if(s[i] == '_') {
                rows.Add(new Field(FieldType.Target));
            }
            else if(s[i] == '\n') {
                levelLayout.Add(rows);
                rows = new List<Field>(); //zerujemy rows
            }
           
        }
        levelLayout.Add(rows); //to jest tylko dla ostatniego wiersza, ktory nie ma "\n"

        //wymiary
        height = levelLayout.Count; //count - for arrays
        width = levelLayout[0].Count;




        //Uzupełnić - co zrobić ze spacją? np 
        //####
        //#..#
        // ##
        //dodać pusty obiekt?
        //Jednak - wymiary są zawsze takie same tzn. wiersze mają tyle samo pól, ale różni się ilość ścian:
        //####
        //#..#
        //####




    }
}
