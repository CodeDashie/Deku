// Created by Crowbar 0.71

$modelname "Hyrule\inside_the_deku_tree\props\log.mdl"

$staticprop

$bodygroup "Body"
{
	studio "inside_the_deku_tree_props_log.smd"
}


$surfaceprop "default"

$contents "solid"

$illumposition 0.15 0.15 3000

$ambientboost

$mostlyopaque

$cdmaterials "Hyrule\inside_the_deku_tree\props\"
$cdmaterials ""

$cbox 0 0 0 0 0 0

$bbox -600 -599.7 0 599.701 600 6000

$definebone "static_prop" "" 0 0 0 0 0 0 0 0 0 0 0 0


$sequence "idle" {
	"log_anims\idle.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
}
