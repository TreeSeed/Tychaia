#!/usr/bin/env python

import bpy
import math
import sys

__all__ = ["register", "unregister", "bl_info" ]

bl_info = {
    "name": "Automatic Render",
    "description": "Allows for automatically rendering different angles of a scene.",
    "author": "James Rhodes",
    "version": (1, 0),
    "blender": (2, 64, 0),
    "location": "Render Properties > Automatic Render",
    "warning": "", # used for warning icon and text in addons panel
    "category": "Render"}

# Operator that handles automatic rendering.
class AUTORENDER_OT_operator(bpy.types.Operator):
    bl_idname = "autorender.operator"
    bl_label = "Perform Render"

    # Define function to set camera data.
    def set_camera_rotation(self, camera, p):
        pi = 3.14159265
        tx = -math.sin(p*(pi/180.0)) * 8
        ty = math.cos(p*(pi/180.0)) * 8
        tz = 5.5
        rx = 63.0
        ry = 0.0
        rz = p - 180
        fov = 50.0
        camera.data.angle = fov*(pi/180.0)
        camera.rotation_mode = "XYZ"
        camera.rotation_euler[0] = rx*(pi/180.0)
        camera.rotation_euler[1] = ry*(pi/180.0)
        camera.rotation_euler[2] = rz*(pi/180.0)
        camera.location.x = tx
        camera.location.y = ty
        camera.location.z = tz
 
    def execute(self, context):
        # Define camera positions.
        positions = [ i for i in range(0, 360, int(context.scene.autorender_steps)) ]

        # Perform rendering.
        i = 0
        scn = context.scene
        #for scn in bpy.data.scenes:
            # Set current scene.
            #bpy.context.screen.scene = scn
            
        # Check property values.
        if context.scene.autorender_pathpattern.find("%s") == -1:
            raise Exception("Auto render path pattern must contain %s!")
            #sys.stderr.flush()
            #return {"FINISHED", "hello!"}
            #eturn {"CANCELLED"}
            
        # Get a reference to a camera in the scene.
        camera = None
        for o in scn.objects:
            if o.type == "CAMERA":
                camera = o
        if camera == None:
            raise Exception("The current scene must contain a camera to perform automatic rendering with!")
        
        # Loop over each camera render.
        for p in positions:
            
            # Set the camera rotation.
            self.set_camera_rotation(camera, p)
        
            # Perform render.
            print("Performing render for scene '" + scn.name + "' at angle " + str(p) + ".")
            scn.render.image_settings.file_format = "PNG"
            scn.render.filepath = context.scene.autorender_pathpattern % (str(i) + "-angle" + str(p))
            bpy.ops.wm.redraw_timer(type='DRAW_WIN_SWAP', iterations=1)
            bpy.ops.render.render()
            bpy.data.images['Render Result'].save_render(filepath=context.scene.autorender_pathpattern % (str(i) + "-angle" + str(p)))
            
        return {"FINISHED"}

# Panel for performing rendering.
class AutomaticRenderPanel(bpy.types.Panel):
    bl_idname = "OBJECT_PT_automatic"
    bl_label = "Automatic Render"
    bl_space_type = 'PROPERTIES'
    bl_region_type = 'WINDOW'
    bl_context = "render"

    def draw(self, context):
        self.layout.operator("autorender.operator")
        self.layout.prop(context.scene, "autorender_pathpattern")
        self.layout.prop(context.scene, "autorender_steps")
        
# Register everything.
def register():
    bpy.types.Scene.autorender_pathpattern = bpy.props.StringProperty(
        default = "D:\Tests.png",
        name = "Path",
        subtype = "FILE_PATH"
        )
    bpy.types.Scene.autorender_steps = bpy.props.EnumProperty(
        items = [("5", "Super Fine (72')", ""),
                 ("15", "Fine (36')", ""),
                 ("45", "Eight-way (8')", ""),
                 ("90", "Four-way (4')", ""),
                 ("180", "Two-way (2')", ""),
                 ("360", "Single (1')", "")],
        name = "Steps",
        default = "45")
    bpy.utils.register_module(__name__)
    
# Unregister evertyhing.
def unregister():
    bpy.utils.unregister_module(__name__)
