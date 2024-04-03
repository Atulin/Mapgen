# Mapgen

Simple organic map generator

## Options

* **Verbose output** — displays every move of every pixel, which slows down the generation *significantly*
* **Start location** — by default, it will be located within the middle of the canvas, this setting lets you overwrite that behavior
* **Output file path** — where the file with the map will be located
* **Pretty print** — if true (default) outputs the map as Unicode block character and spaces. Otherwise, outputs 1s and 0s
* **Open file** — opens the file in the default text editor

## Example output

* Map size **50 x 50**
* Start at **33 x 38**
* Pretty print

```
                                                  
                                                  
                                                  
                                                  
                             █                    
                             █                    
                           ████                   
                              ███                 
                             ██                   
                           ████                   
                          ███ ██████              
                         ██ █    ██  █  ██  █     
                 ████    █     ███   █  █  ████   
                   █             ██  ███████      
                  ██             ██ ██            
                   ██            ██ ██ █  ██      
               █ ██ █             ███  ██████     
              ███████   ████  ██  ████ ██         
              ██  █ ██     ██ ██   ██  █          
             ██   ████      █  █   █  ██          
                  █  █      ██ █   ██  ██ █ █     
                   █ █████████████  ██████████    
                   ██████ █  █   █  █   █   ██    
                          █    █ ██ ██            
                               ███████            
                                █  █              
                               ██  ██             
                       █████████████  █           
                       ███   ██ █ █████           
                              █     ███           
                       █           ███            
                       ██           ██   ██       
             █         ███           ██████       
            ██          ████        ███████       
            ██            ██  █ █   ████  ██      
         █████  █     █  ██   █ ███ █  █          
             █████ █  █ ███  ████ ███  ██         
           █ █  ███████   ████ █████   ████       
           ███  █  ████████ █  ██  █   █ █        
              ███    ██ ██  █  ██     ██          
             ██ █  █████ ████  █     ███          
            ██  ██   █   █  █          █          
            ██   █   █              ███████       
                 ██ ██                ███████     
                 █   █                      ██    
                 █                           █    
                 █                           █    
                                                  
                                                  
                                                  


```