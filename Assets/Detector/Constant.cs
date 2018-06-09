public class Constant
{
    //Value default for kinectv2
    public static int KINECT_IR_WIDTH = 512;
    public static int KINECT_IR_HEIGHT = 424;

    public readonly static int LEFT_X_FEED_DETECTOR = 70;
    public readonly static int RIGHT_X_FEED_DETECTOR = 450;
    public readonly static int TOP_Y_FEED_DETECTOR = 40;
    public readonly static int BOTTOM_Y_FEED_DETECTOR = 330;

    //Depend on image feed detector, if size feed detector changing --> must measure screen projector
    public readonly static int TOPLEFT_X_SCREEN_PROJECTOR = 72;
    public readonly static int TOPLEFT_Y_SCREEN_PROJECTOR = 220;
    public readonly static int RIGHTBOTTOM_X_SCREEN_PROJECTOR = 291;
    public readonly static int RIGHTBOTTOM_Y_SCREEN_PROJECTOR = 50;

    /*public readonly static int LEFT_X_FEED_DIRECTOR = 0;
    public readonly static int RIGHT_X_FEED_DIRECTOR = 512;
    public readonly static int TOP_Y_FEED_DIRECTOR = 0;
    public readonly static int BOTTOM_Y_FEED_DIRECTOR = 424;*/

    public readonly static int FEED_DETECTOR_WIDTH = RIGHT_X_FEED_DETECTOR - LEFT_X_FEED_DETECTOR;
    public readonly static int FEED_DETECTOR_HEIGHT = BOTTOM_Y_FEED_DETECTOR - TOP_Y_FEED_DETECTOR;

    public readonly static int MAX_ENTRY_DATA_COLLECT = 90;
}
