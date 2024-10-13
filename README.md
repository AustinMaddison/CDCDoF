# Circular Separable Convolutional Lens Blur (CocLb)
![alt text for screen readers](/Images/cover.png)
## Results
The effects results can be found in Slides.pdf

## Proposal Summary 
The project proposed the implementation of Circular Separable Convolutional Depth of Field (CocDof), a post-processing technique originating from Electronic Arts. This technique addresses the challenge of rendering bokeh depth of field efficiently, particularly when dealing with a large Circle of Confusion. Traditional methods for handling a very large Circle of Confusion are computationally expensive. What makes this cicular convolution different is that it is seperable. This in turn reduces the run tume from O(M^2 x N^2) to O(M^2 x N). The plan is to implement this algorithm using GPU fragment shaders into Unity's real-time rendering engine, leveraging HLSL and C#. The intention was to integrate it as a post-processing effect within a practical program, capitalizing on the parallel processing capabilities of GPU architecture.

![image](https://github.com/cs-muic/2023-t1-finalproject-image-lens-blur-effect/assets/23569282/96234302-3a03-4bf0-b3b7-d0d69f33d8a1)
![image](https://github.com/cs-muic/2023-t1-finalproject-image-lens-blur-effect/assets/23569282/b67120ad-1d9e-4b9a-89c8-b94bcab4979d)


## Project Summary 
As the project unfolded, the realization dawned that implementing a full-depth-of-field system is intricate, involving considerations of various physical camera settings like aperture, focal length, and sensor size to define an appropriate mask for the plane of focus. Consequently, the project scope was refined to focus on the core blurring algorithm, opting for Lens Blur over full Depth of Field. Initial steps involved becoming acquainted with Unity's render pipeline, including the creation of render targets essential for achieving the desired effect.

## Measuring Success
I sourced a bunch of reference of the lens blur effect both from existing implmentations and from photography. Like most things in computer graphics as long as it looks plausible its a success, doesnt need to be neccesarily physically correct.
![image](https://github.com/cs-muic/2023-t1-finalproject-image-lens-blur-effect/assets/23569282/486d9327-62fd-4eb7-a1ce-9b464cf5da7a)
![image](https://github.com/cs-muic/2023-t1-finalproject-image-lens-blur-effect/assets/23569282/04832196-213f-487d-8cbc-e3cc8954287f)


## MATLAB Implementation
Before attempting this project I ensured that this technque of lens blur works. So I used MATLAB's GPU arrays to compute what I would expect from the implementation described in the paper. It indeed worked! 
![image](https://github.com/cs-muic/2023-t1-finalproject-image-lens-blur-effect/assets/23569282/93d32e65-d135-428e-88b2-fd0dbbcc1f16)

## Unity Implementation  
### Gaussian Blur
The initial focus was on Gaussian Blur, chosen as the foundational element for CocLb. Both techniques require the construction of a kernel and the execution of convolutions in two separate passes. The approach involved specifying in C# where in Unity's rendering pipeline to intervene, describing the necessary passes, buffers, and uniforms for the shader, and then crafting the shader itself. This resulted in a real-time Gaussian blur with variable blur size and kernel precision. Notably, the Gaussian weights used in the kernel for convolution were dynamically calculated in the shader rather than being precomputed. Upon completion of the passes, the buffer was read into the main render target, which encompasses Unity's Viewport and Cameras.

### Circular Separable Convolutional Lens Blur
The original plan was to repurpose the Gaussian Blur as the foundation for CocLb. However, challenges arose as CocLb required additional buffers, one for each real and imaginary component to reconstruct the Circular kernel. I ended up using 2 components. This necessitated a single pass to generate four more passes. Unfortunately, Unity's HLSL proved to be less flexible, requiring workarounds and custom render target handlers. However at the end I was able to overcome this obstacle using render textures, they are very expensive because render pass buffers have to be brought back to the CPU. This is the only workaround I could figure out. 

![image](https://github.com/cs-muic/2023-t1-finalproject-image-lens-blur-effect/assets/23569282/252e2279-47fe-4dc7-941b-0710025de34a)
![image](https://github.com/cs-muic/2023-t1-finalproject-image-lens-blur-effect/assets/23569282/12747815-090e-48dc-9c63-d65263e76e29)
![image](https://github.com/cs-muic/2023-t1-finalproject-image-lens-blur-effect/assets/23569282/2d61d763-9cfa-4c25-b687-ae033590d9b0)
![image](https://github.com/cs-muic/2023-t1-finalproject-image-lens-blur-effect/assets/23569282/3e3c4dac-84a2-4864-a434-a493606cc6d9)
![image](https://github.com/cs-muic/2023-t1-finalproject-image-lens-blur-effect/assets/23569282/04ba806c-e4b5-4622-aa99-670f38c1300d)
![image](https://github.com/cs-muic/2023-t1-finalproject-image-lens-blur-effect/assets/23569282/b0967e4f-4ed4-42d7-b88f-171184accf13)
![image](https://github.com/cs-muic/2023-t1-finalproject-image-lens-blur-effect/assets/23569282/64fa27f6-1190-40b2-87c2-ad533985f9ff)





