import java.io.*;

public class Gear {

	public static void main(String[] args) {
		try {
            //  open up standard input
            BufferedReader br = new BufferedReader(new InputStreamReader(System.in));

            int num_teeth = 0, num_points = 0;
            double dia_pitch=0.0, press_angle=0.0, module = 0.0, profile_shift = 0.0;
            String type = "";
            
            //  read the values from the command-line; need to use try/catch with the
            //  readLine() method
            if (args.length > 0) {
                type = args[0];
            }
            else {
                System.out.print("Metric (module) or Standard (diametral pitch)? (M/S): ");
                type = br.readLine();
            }

            if (type.compareToIgnoreCase("S") == 0)
            {
                if (args.length > 1) {
                    dia_pitch = Double.parseDouble(args[1]);
                }
                else {
                    System.out.print("Enter Diametral Pitch (teeth/inch, commonly 48, 32, 24, 20, 18, 16, or 12): ");
                    dia_pitch = Double.parseDouble(br.readLine());
                }
                module = 25.4/dia_pitch;
            }
            else if (type.compareToIgnoreCase("M")==0)
            {
                if (args.length > 1) {
                    module = Double.parseDouble(args[1]);
                }
                else {
                    System.out.print("Enter Module (teeth/mm, commonly 0.5, 0.8, 1.00, 1.25, 1.50, 2.50, or 3 ): ");
                    module = Double.parseDouble(br.readLine());
                }
                dia_pitch = 25.4/module;
            }
            else 
            {
                System.out.println("Error: please enter either 'M' for metric or 'S' for standard");
                System.exit(1);
            }
            System.out.print("Module = " + module + "\nPitch = " + dia_pitch + "\n");
            double scaleVal = 1.0;
            if (type.compareToIgnoreCase("S") == 0)
                scaleVal = 1.0/25.4;

            double inputAngle = 0;
            if (args.length > 2) {
                inputAngle = Double.parseDouble(args[2]);
            }
            else {
                System.out.print("Enter Pressure Angle in degrees (commonly 14.5, 20, or 25): ");
                inputAngle = Double.parseDouble(br.readLine());
            }
            press_angle = Math.PI/180.0*inputAngle;

            if (args.length > 3) {
                num_teeth = Integer.parseInt(args[3]);
            }
            else {
                System.out.print("Enter Number of Teeth: ");
                num_teeth = Integer.parseInt(br.readLine());
            }
            
            double min_shift = Math.max( -0.5,(double)(30.0-num_teeth)/40.0);
            double max_shift = 0.6;
            if (args.length > 4) {
                profile_shift = Double.parseDouble(args[4]);
            }
            else {
                System.out.print("Enter Profile Shift (reccomended [" + min_shift + ", " + max_shift + "]): ");
                profile_shift = Double.parseDouble(br.readLine());
            }

            if (args.length > 5) {
                num_points = Integer.parseInt(args[5]);
            }
            else {
                System.out.print("Enter Number of Output Points for Involute and Fillet Segments: ");
                num_points = Integer.parseInt(br.readLine());
            }
            
            
            double pitch_dia=0,base_dia=0,ad=0,ded=0,work_dep=0,whole_dep=0,clear=0,od=0,tip_rad=0;
            pitch_dia = module * num_teeth; // Pitch Diameter = Module * Teeth

            base_dia = pitch_dia*Math.cos(press_angle); //Base Circle Diameter = Pitch Diameter × Cosine(Pressure Angle)

            ad = module; //Addendum = Module

            ded = 1.157*module; //Dedendum = 1.157 × Module

            work_dep = 2*module; //Working Depth = 2 × Module

            whole_dep = 2.157*module; //Whole Depth = 2.157 × Module

            od = module*(num_teeth+2); //Outside Diameter = Module × (Teeth + 2)

            tip_rad = 0.25*module; //Tip Radius = 0.25 x Module

            double[] theta = new double[2*num_points];
            double[] x = new double[2*num_points];
            double[] y = new double[2*num_points];
            double[] z = new double[2*num_points];
            double theta_min=0, theta_max=0, theta_inc=0, U=0, V=0;

            // Generate Involute Points
            U = -(Math.PI/4.0+(1.0-0.25)*Math.tan(press_angle)+0.25/Math.cos(press_angle));
            V =  0.25-1.0;
            theta_min = 2.0/num_teeth*(U+(V+profile_shift)*1.0/Math.tan(press_angle));
            theta_max = 1.0/(num_teeth*Math.cos(press_angle))*Math.sqrt(Math.pow((2+num_teeth+2*profile_shift),2)-Math.pow(num_teeth*Math.cos(press_angle),2))-(1+2*profile_shift/num_teeth)*Math.tan(press_angle)-Math.PI/(2.0*num_teeth);
            theta_inc = (theta_max-theta_min)/num_points;

            for (int i = 0; i < num_points; i++) {
                theta[i] = theta_min+theta_inc*i;
                x[i] = 	num_teeth*module/2.0*(Math.sin(theta[i])-((theta[i]+Math.PI/(2.0*num_teeth))*Math.cos(press_angle)+2*profile_shift/num_teeth*Math.sin(press_angle))*Math.cos(theta[i]+press_angle));
                y[i] = num_teeth*module/2.0*(Math.cos(theta[i])+((theta[i]+Math.PI/(2.0*num_teeth))*Math.cos(press_angle)+2*profile_shift/num_teeth*Math.sin(press_angle))*Math.sin(theta[i]+press_angle));
                z[i] = 0.0;
            }
            
            double land = Math.abs(2.0*x[num_points-1]); //Define the land distance

            // Generate Fillet Points
            double P=0,Q=0,L=0;
            theta_min = 2.0/num_teeth*(U+(V+profile_shift)/Math.tan(press_angle));
            theta_max = 2.0*U/num_teeth;
            theta_inc = (theta_max-theta_min)/num_points;
            for (int i=num_points; i < 2*num_points; i++) 
            {
                theta[i] = theta_min+theta_inc*(i-num_points);
                L = Math.sqrt(1+4*Math.pow((V+profile_shift)/(2*U-num_teeth*theta[i]),2));
                Q = 2*0.25/L*(V+profile_shift)/(2*U-num_teeth*theta[i])+V+num_teeth/2.0+profile_shift;
                P = 0.25/L+(U-num_teeth*theta[i]/2.0);
                x[i] = module*(P*Math.cos(theta[i])+Q*Math.sin(theta[i]));
                y[i] = module*(-P*Math.sin(theta[i])+Q*Math.cos(theta[i]));
                z[i] = 0.0;
            }

            int out_form = 0;
            String out_name = "";

            out_form = 0;
            while (out_form <= 0) {
                String ofname;
                if (args.length > 6 && out_form == 0) {
                    ofname = args[6];
                }
                else {
                    System.out.print("Enter Output Format\ntsv, ibl, csv: ");
                    ofname = br.readLine();
                    if (ofname == null) {
                        break;
                    }
                }
                if (ofname.compareToIgnoreCase("tsv") == 0) {
                    out_form = 1;
                }
                else if (ofname.compareToIgnoreCase("ibl") == 0) {
                    out_form = 2;
                }
                else if (ofname.compareToIgnoreCase("csv") == 0) {
                    out_form = 3;
                }
                else {
                    System.out.print("'" + ofname + "' is not a valid file format.\n");
                    out_form = -1;
                }
            }
            
            /*
            System.out.print("Enter Output File Name (no suffix): ");
            try {
                out_name = br.readLine();
            } catch (IOException ioe) {
                System.out.println("IO error");
                System.exit(1);
            }
            */
            out_name = type + "_" + Integer.toString((int)dia_pitch) + "_" + Double.toString(inputAngle) 
                + "_" + Integer.toString((int)num_teeth) 
                + "_" + Double.toString(profile_shift) + "_" + Integer.toString((int)num_points);
            String param_name = out_name + "_params.txt";
            if (out_form == 2) 
            {
                out_name = out_name + ".ibl";
                try{
                    // Create file 
                    FileWriter fstream = new FileWriter(out_name);
                    BufferedWriter out = new BufferedWriter(fstream);
                    out.write("Open	Index	Arclength\n");
                    out.write("Begin section ! 1\n");
                    out.write("Begin curve ! 1\n");
                    for (int i = 0; i < num_points; i++) {
                        out.write((i+1) + "\t" + x[i]*scaleVal + "\t" + y[i]*scaleVal + "\t" + z[i]*scaleVal + "\n");
                    }
                    out.write("Begin section ! 2\n");
                    out.write("Begin curve ! 2\n");
                    for (int i = num_points; i < 2*num_points; i++) {
                        out.write((i-num_points+1) + "\t" + x[i]*scaleVal + "\t" + y[i]*scaleVal + "\t" + z[i]*scaleVal + "\n");
                    }
                    out.close();
                    System.out.println("Wrote all points to " + out_name);
                    }catch (Exception e){//Catch exception if any
                      System.err.println("Error: " + e.getMessage());
                }
            }
            else if (out_form == 3)
            {
                out_name = out_name + ".csv";
                try{
                    // Create file 
                    FileWriter fstream = new FileWriter(out_name);
                    BufferedWriter out = new BufferedWriter(fstream);
                    if (type.compareToIgnoreCase("S") == 0) {
                        out.write("in\n");
                    }
                    else {
                        out.write("mm\n");
                    }
                    out.write("X,Y,Z\n");
                    for (int i = 0; i < num_points; i++) {
                        out.write( x[i]*scaleVal + "," + y[i]*scaleVal + "," + z[i]*scaleVal + "\n");
                    }
                    out.write("0,0,0\n");
                    for (int i = num_points; i < 2*num_points; i++) {
                        out.write( x[i]*scaleVal + "," + y[i]*scaleVal + "," + z[i]*scaleVal + "\n");
                    }
                    out.close();
                    System.out.println("Wrote all points to " + out_name);
                }
                catch (Exception e){//Catch exception if any
                      System.err.println("Error: " + e.getMessage());
                }
            }
            else
            {
                out_name = out_name + ".tsv";
                try{
                    // Create file 
                    FileWriter fstream = new FileWriter(out_name);
                    BufferedWriter out = new BufferedWriter(fstream);
                    for (int i = 0; i < 2*num_points; i++) {
                        out.write( x[i]*scaleVal + "\t" + y[i]*scaleVal + "\t" + z[i]*scaleVal + "\n");
                    }
                    out.close();
                    System.out.println("Wrote all points to " + out_name);
                }
                catch (Exception e){//Catch exception if any
                      System.err.println("Error: " + e.getMessage());
                }
            }	

            try{
                // Create file 
                FileWriter fstream = new FileWriter(param_name);
                BufferedWriter out = new BufferedWriter(fstream);
                out.write("pitch diameter:\t" + pitch_dia*scaleVal + "\n");
                out.write("extended pitch diameter: \t" + (pitch_dia+2*profile_shift) + "\n");
                out.write("base diameter:\t" + base_dia*scaleVal + "\n");
                out.write("addendum:\t" + ad*scaleVal + "\n");
                out.write("dedendum:\t" + ded*scaleVal + "\n");
                out.write("working depth:\t" + work_dep*scaleVal + "\n");
                out.write("whole depth:\t" + whole_dep*scaleVal + "\n");
                out.write("outer diameter:\t" + od*scaleVal + "\n");
                out.write("tip radius:\t" + tip_rad*scaleVal + "\n");
                out.write("land:\t" + land*scaleVal + "\n");
                out.close();
                System.out.println("Wrote all parameters to " + param_name);
            }catch (Exception e){//Catch exception if any
                  System.err.println("Error: " + e.getMessage());
            }
		} catch (IOException ioe) {
			System.out.println("IO error");
			System.exit(1);
		}	
	}
}












