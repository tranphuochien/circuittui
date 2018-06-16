public class Constant
{
    //Value default for kinectv2
    public static int KINECT_IR_WIDTH = 512;
    public static int KINECT_IR_HEIGHT = 424;

    /*public readonly static int LEFT_X_FEED_DETECTOR = 70;
    public readonly static int RIGHT_X_FEED_DETECTOR = 450;
    public readonly static int TOP_Y_FEED_DETECTOR = 40;
    public readonly static int BOTTOM_Y_FEED_DETECTOR = 330;*/

    public readonly static int LEFT_X_FEED_DETECTOR = 0;
    public readonly static int RIGHT_X_FEED_DETECTOR = 450;
    public readonly static int TOP_Y_FEED_DETECTOR = 10;
    public readonly static int BOTTOM_Y_FEED_DETECTOR = 360;

    //Depend on image feed detector, if size feed detector changing --> must measure screen projector
    public readonly static int TOPLEFT_X_SCREEN_PROJECTOR = 123;
    public readonly static int TOPLEFT_Y_SCREEN_PROJECTOR = 279;
    public readonly static int RIGHTBOTTOM_X_SCREEN_PROJECTOR = 384;
    public readonly static int RIGHTBOTTOM_Y_SCREEN_PROJECTOR = 72;

    public readonly static string TOKEN_BEGIN_POSITION = "0001:";
    public readonly static string TOKEN_BEGIN_URL = "0002:";
    public const string TOKEN_BEGIN_SHAKE = "0003";
    public const string TOKEN_BEGIN_FREEZE = "0004";
    public const string TOKEN_BEGIN_FLIP = "0005";
    public const string TOKEN_BEGIN_ZOOM = "0006";
    public const string TOKEN_BEGIN_ZOOMDEFAULT = "0007";
    public const string TOKEN_BEGIN_DROP = "0008";
    public const string TOKEN_BEGIN_GET = "0009";
    public const string TOKEN_BEGIN_SET_FLAG = "0010";
    public const string TOKEN_BEGIN_ANALYZE = "0011";
    public readonly static string TOKEN_SPLIT = "|";
    public readonly static string TOKEN_END = "@";

    public readonly static int FEED_DETECTOR_WIDTH = RIGHT_X_FEED_DETECTOR - LEFT_X_FEED_DETECTOR;
    public readonly static int FEED_DETECTOR_HEIGHT = BOTTOM_Y_FEED_DETECTOR - TOP_Y_FEED_DETECTOR;

    public readonly static int MAX_ENTRY_DATA_COLLECT = 50;
}
