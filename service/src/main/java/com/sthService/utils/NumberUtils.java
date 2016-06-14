package com.sthService.utils;

/**
 * Created by jakub on 14.06.2016.
 */
public class NumberUtils {

    public static boolean areIntegers(String... number) {
        try {
            for (String s : number) {
                int num = Integer.parseInt(s);
            }
        } catch (NumberFormatException e) {
            return false;
        }

        return true;
    }
}
